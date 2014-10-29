// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.ResolveStateMachine;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Persist;
using NakedObjects.Core.Util;
using NakedObjects.Util;

namespace NakedObjects.Core.Adapter {
    public class PocoAdapter : INakedObject {
        private static readonly ILog Log;
        private string defaultTitle;
        private IOid oid;
        private readonly IMetamodelManager metamodel;
        private readonly ISession session;
        private readonly IObjectPersistor persistor;
        private readonly INakedObjectManager manager;
        private readonly ILifecycleManager lifecycleManager;
        private object poco;
        private IObjectSpec spec;
        private ITypeOfFacet typeOfFacet;
        private IVersion version;

        static PocoAdapter() {
            Log = LogManager.GetLogger(typeof (PocoAdapter));
        }

        public PocoAdapter(IMetamodelManager metamodel, ISession session, IObjectPersistor persistor, ILifecycleManager lifecycleManager, INakedObjectManager nakedObjectManager, object poco, IOid oid) {
            Assert.AssertNotNull(metamodel);
            //Assert.AssertNotNull(session);

            if (poco is INakedObject) {
                throw new AdapterException("Adapter can't be used to adapt an adapter: " + poco);
            }
            this.metamodel = metamodel;
            this.session = session;
            this.persistor = persistor;
            this.manager = nakedObjectManager;
            this.lifecycleManager = lifecycleManager;

            this.poco = poco;
            this.oid = oid;
            ResolveState = new ResolveStateMachine(this, session);
            version = new NullVersion();
        }

        protected internal virtual string DefaultTitle {
            get { return defaultTitle; }
        }

        #region INakedObject Members

        public virtual ITypeOfFacet TypeOfFacet {
            get {
                if (typeOfFacet == null) {
                    return Spec.GetFacet<ITypeOfFacet>();
                }
                return typeOfFacet;
            }

            set { typeOfFacet = value; }
        }

        public virtual object Object {
            get { return poco; }
        }

        /// <summary>
        ///     Returns the name of the icon to use to represent this object
        /// </summary>
        public virtual string IconName() {
            return Spec.GetIconName(this);
        }

        public IResolveStateMachine ResolveState { get; private set; }

        public virtual IObjectSpec Spec {
            get {
                if (spec == null) {
                    spec = metamodel.GetSpecification(Object.GetType());
                    defaultTitle = "A" + (" " + spec.SingularName).ToLower();
                }
                return spec;
            }
        }

        public virtual IVersion Version {
            get { return version; }
        }

        public virtual IVersion OptimisticLock {
            set {
                if (ShouldSetVersion(value)) {
                    version = value;
                }
            }
        }

        /// <summary>
        ///     Returns the title from the underlying business object. If the object has not yet been resolved the
        ///     specification will be asked for a unresolved title, which could of been persisted by the persistence
        ///     mechanism. If either of the above provides null as the title then this method will return a title
        ///     relating to the name of the object type, e.g. "A Customer", "A Product".
        /// </summary>
        public virtual string TitleString() {
            try {
                if (Spec.IsCollection && !Spec.IsParseable) {
                    return CollectionTitleString(Spec.GetFacet<ICollectionFacet>());
                }
                return Spec.GetTitle(this) ?? DefaultTitle;
            }
            catch (Exception e) {
                Log.Error("Exception on ToString", e);
                throw new TitleException("Exception on ToString POCO: " + (poco == null ? "unknown" : poco.GetType().FullName), e);
            }
        }


        public virtual string InvariantString() {

            return Spec.GetInvariantString(this);
        }


        /// <summary>
        ///     Sometimes it is necessary to manage the replacement of the underlying domain object (by another
        ///     component such as an object store). This method allows the adapter to be kept while the domain object
        ///     is replaced.
        /// </summary>
        public virtual void ReplacePoco(object obj) {
            poco = obj;
        }

        public string ValidToPersist() {
            IAssociationSpec[] properties = Spec.Properties;
            foreach (IAssociationSpec property in properties) {
                INakedObject referencedObject = property.GetNakedObject(this);
                if (property.IsUsable(this).IsAllowed && property.IsVisible(this)) {
                    if (property.IsMandatory && property.IsEmpty(this)) {
                        return string.Format(Resources.NakedObjects.PropertyMandatory, spec.ShortName, property.GetName());
                    }
                    if (property.IsObject) {
                        IConsent valid = ((IOneToOneAssociationSpec) property).IsAssociationValid(this, referencedObject);
                        if (valid.IsVetoed) {
                            return string.Format(Resources.NakedObjects.PropertyInvalid, spec.ShortName, property.GetName(), valid.Reason);
                        }
                    }
                }
                if (property.IsObject) {
                    if (referencedObject != null && referencedObject.ResolveState.IsTransient()) {
                        string referencedObjectMessage = referencedObject.ValidToPersist();
                        if (referencedObjectMessage != null) {
                            return referencedObjectMessage;
                        }
                    }
                }
            }

            var validateFacet = spec.GetFacet<IValidateObjectFacet>();
            return validateFacet == null ? null : validateFacet.Validate(this);

            //foreach (INakedObjectValidation validator in spec.ValidateMethods()) {
            //    IEnumerable<INakedObject> parameters = validator.ParameterNames.Select(name => spec.Properties.Single(p => p.Id.ToLower() == name).GetNakedObject(this));
            //    string result = validator.Execute(this, parameters.ToArray());
            //    if (result != null) {
            //        return result;
            //    }
            //}

            //return null;
        }

        public void SetATransientOid(IOid newOid) {
            Assert.AssertNull("Oid must be null", oid);
            Assert.AssertTrue("New Oid must be transient", newOid.IsTransient);
            oid = newOid;
        }

        public virtual void CheckLock(IVersion otherVersion) {
            if (version != null && version.IsDifferent(otherVersion)) {
                Log.Info("concurrency conflict on " + this + " (" + otherVersion + ")");
                throw new ConcurrencyException(this, otherVersion);
            }
        }

        public virtual IOid Oid {
            get { return oid; }
            protected set {
                if (value == null) {
                    throw new NullReferenceException();
                }
                oid = value;
            }
        }

        #endregion

        private string CollectionTitleString(ICollectionFacet facet) {
            int size = ElementsLoaded() ? facet.AsEnumerable(this, manager).Count() : CollectionUtils.IncompleteCollection;
            var elementSpecification = TypeOfFacet == null ? null :  metamodel.GetSpecification(TypeOfFacet.GetValueSpec(this));
            return CollectionUtils.CollectionTitleString(elementSpecification, size);
        }

        private bool ElementsLoaded() {
            return ResolveState.IsTransient() || ResolveState.IsResolved();
        }

        public override string ToString() {
            var str = new AsString(this);
            ToString(str);

            // don't do title of unresolved objects as this may force the resolving of the object.
            if (ResolveState.IsTransient() || ResolveState.IsResolved() || ResolveState.IsAggregated()) {
                str.Append("title", TitleString());
            }
            str.AppendAsHex("poco-hash", poco.GetHashCode());
            return str.ToString();
        }

        private bool ShouldSetVersion(IVersion newVersion) {
            return newVersion.IsDifferent(version);
        }

        protected internal virtual void ToString(AsString str) {
            str.Append(ResolveState.CurrentState.Code);

            if (Oid != null) {
                str.Append(":");
                str.Append(Oid.ToString());
            }
            else {
                str.Append(":-");
            }
            str.AddComma();
            if (spec == null) {
                str.Append("class", Object.GetType().FullName);
            }
            else {
                str.Append("specification", spec.ShortName);
                str.Append("Type", spec.FullName);
            }

            if (Object != null && TypeUtils.IsProxy(Object.GetType())) {
                str.Append("proxy", Object.GetType().FullName);
            }
            else {
                str.Append("proxy", "None");
            }

            str.Append("version", version == null ? null : version.AsSequence());
        }

        public  void LoadAnyComplexTypes() {          
            if (Spec.IsService ||
                Spec.IsViewModel ||
                Spec.ContainsFacet(typeof(IComplexTypeFacet))) {
                return;
            }
            persistor.LoadComplexTypes(this, ResolveState.IsGhost());
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}