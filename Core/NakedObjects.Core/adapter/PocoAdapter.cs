// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Diagnostics;
using System.Linq;
using Common.Logging;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Resolve;
using NakedObjects.Core.Util;
using NakedObjects.Util;

namespace NakedObjects.Core.Adapter {
    internal class PocoAdapter : INakedObject {
        private static readonly ILog Log;
        private readonly ILifecycleManager lifecycleManager;
        private readonly INakedObjectManager nakedObjectManager;
        private readonly IMetamodelManager metamodel;
        private readonly IObjectPersistor persistor;
        private readonly ISession session;
        private string defaultTitle;
        private IOid oid;
        private object poco;
        private ITypeSpec spec;
        private ITypeOfFacet typeOfFacet;
        private IVersion version;

        static PocoAdapter() {
            Log = LogManager.GetLogger(typeof (PocoAdapter));
        }

        public PocoAdapter(IMetamodelManager metamodel, ISession session, IObjectPersistor persistor, ILifecycleManager lifecycleManager, INakedObjectManager nakedObjectManager, object poco, IOid oid) {
            Assert.AssertNotNull(metamodel);
            Assert.AssertNotNull(session);

            if (poco is INakedObject) {
                throw new AdapterException("Adapter can't be used to adapt an adapter: " + poco);
            }
            this.metamodel = metamodel;
            this.session = session;
            this.persistor = persistor;
            this.nakedObjectManager = nakedObjectManager;
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

        public virtual ITypeSpec Spec {
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
            var objectSpec = Spec as IObjectSpec;
            Trace.Assert(objectSpec != null);

            IAssociationSpec[] properties = objectSpec.Properties;
            foreach (IAssociationSpec property in properties) {
                INakedObject referencedObject = property.GetNakedObject(this);
                if (property.IsUsable(this).IsAllowed && property.IsVisible(this)) {
                    if (property.IsMandatory && property.IsEmpty(this)) {
                        return string.Format(Resources.NakedObjects.PropertyMandatory, objectSpec.ShortName, property.Name);
                    }
                    var associationSpec = property as IOneToOneAssociationSpec;
                    if (associationSpec != null) {
                        IConsent valid = associationSpec.IsAssociationValid(this, referencedObject);
                        if (valid.IsVetoed) {
                            return string.Format(Resources.NakedObjects.PropertyInvalid, objectSpec.ShortName, associationSpec.Name, valid.Reason);
                        }
                    }
                }
                if (property is IOneToOneAssociationSpec) {
                    if (referencedObject != null && referencedObject.ResolveState.IsTransient()) {
                        string referencedObjectMessage = referencedObject.ValidToPersist();
                        if (referencedObjectMessage != null) {
                            return referencedObjectMessage;
                        }
                    }
                }
            }

            var validateFacet = objectSpec.GetFacet<IValidateObjectFacet>();
            return validateFacet == null ? null : validateFacet.Validate(this);
        }

        public void SetATransientOid(IOid newOid) {
            Assert.AssertNull("Oid must be null", oid);
            Assert.AssertTrue("New Oid must be transient", newOid.IsTransient);
            oid = newOid;
        }

        public virtual void CheckLock(IVersion otherVersion) {
            if (version != null && version.IsDifferent(otherVersion)) {
                Log.Info("concurrency conflict on " + this + " (" + otherVersion + ")");
                throw new ConcurrencyException(this);
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

        public void LoadAnyComplexTypes() {
            if (Spec is IServiceSpec ||
                Spec.IsViewModel ||
                Spec.ContainsFacet(typeof (IComplexTypeFacet))) {
                return;
            }
            persistor.LoadComplexTypes(this, ResolveState.IsGhost());
        }

        public void Created() {
            CallCallback<ICreatedCallbackFacet>();
        }

        public void Deleting() {
            CallCallback<IDeletingCallbackFacet>();
        }

        public void Deleted() {
            CallCallback<IDeletedCallbackFacet>();
        }

        public void Loading() {
            CallCallback<ILoadingCallbackFacet>();
        }

        public void Loaded() {
            CallCallback<ILoadedCallbackFacet>();
        }

        public void Persisting() {
            CallCallback<IPersistingCallbackFacet>();
        }

        public void Persisted() {
            CallCallback<IPersistedCallbackFacet>();
        }

        public void Updating() {
            CallCallback<IUpdatingCallbackFacet>();
        }

        public void Updated() {
            CallCallback<IUpdatedCallbackFacet>();
        }

        #endregion

        private string CollectionTitleString(ICollectionFacet facet) {
            int size = ElementsLoaded() ? facet.AsEnumerable(this, nakedObjectManager).Count() : CollectionUtils.IncompleteCollection;
            var elementSpecification = (IObjectSpec) (TypeOfFacet == null ? null : metamodel.GetSpecification(TypeOfFacet.GetValueSpec(this, metamodel.Metamodel)));
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

        private void ToString(AsString str) {
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

        private void CallCallback<T>() where T : ICallbackFacet {
            Spec.GetFacet<T>().Invoke(this, session, lifecycleManager, metamodel);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}