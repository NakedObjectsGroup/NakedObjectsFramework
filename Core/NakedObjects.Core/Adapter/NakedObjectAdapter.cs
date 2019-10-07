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
    public sealed class NakedObjectAdapter : INakedObjectAdapter {
        private static readonly ILog Log = LogManager.GetLogger(typeof(NakedObjectAdapter));
        private readonly ILifecycleManager lifecycleManager;
        private readonly IMetamodelManager metamodel;
        private readonly INakedObjectManager nakedObjectManager;
        private readonly IObjectPersistor persistor;
        private readonly ISession session;
        private ITypeSpec spec;

        public NakedObjectAdapter(IMetamodelManager metamodel, ISession session, IObjectPersistor persistor, ILifecycleManager lifecycleManager, INakedObjectManager nakedObjectManager, object poco, IOid oid) {
            Assert.AssertNotNull(metamodel);
            Assert.AssertNotNull(session);

            if (poco is INakedObjectAdapter) {
                throw new AdapterException(Log.LogAndReturn($"Adapter can't be used to adapt an adapter: {poco}"));
            }

            this.metamodel = metamodel;
            this.session = session;
            this.persistor = persistor;
            this.nakedObjectManager = nakedObjectManager;
            this.lifecycleManager = lifecycleManager;

            this.Object = poco;
            Oid = oid;
            ResolveState = new ResolveStateMachine(this, session);
            Version = new NullVersion();
        }

        private string DefaultTitle { get; set; }

        private ITypeOfFacet TypeOfFacet => Spec.GetFacet<ITypeOfFacet>();

        #region INakedObjectAdapter Members

        public object Object { get; private set; }

        /// <summary>
        ///     Returns the name of the icon to use to represent this object
        /// </summary>
        public string IconName() {
            return Spec.GetIconName(this);
        }

        public IResolveStateMachine ResolveState { get; private set; }

        public ITypeSpec Spec {
            get {
                if (spec == null) {
                    spec = metamodel.GetSpecification(Object.GetType());
                    DefaultTitle = "A" + (" " + spec.SingularName).ToLower();
                }

                return spec;
            }
        }

        public IVersion Version { get; private set; }

        public IVersion OptimisticLock {
            set {
                if (ShouldSetVersion(value)) {
                    Version = value;
                }
            }
        }

        /// <summary>
        ///     Returns the title from the underlying business object. If the object has not yet been resolved the
        ///     specification will be asked for a unresolved title, which could of been persisted by the persistence
        ///     mechanism. If either of the above provides null as the title then this method will return a title
        ///     relating to the name of the object type, e.g. "A Customer", "A Product".
        /// </summary>
        public string TitleString() {
            try {
                if (Spec.IsCollection && !Spec.IsParseable) {
                    return CollectionTitleString(Spec.GetFacet<ICollectionFacet>());
                }

                return Spec.GetTitle(this) ?? DefaultTitle;
            }
            catch (Exception e) {
                throw new TitleException(Log.LogAndReturn("Exception on ToString POCO: " + (Object == null ? "unknown" : Object.GetType().FullName)), e);
            }
        }

        public string InvariantString() {
            return Spec.GetInvariantString(this);
        }

        /// <summary>
        ///     Sometimes it is necessary to manage the replacement of the underlying domain object (by another
        ///     component such as an object store). This method allows the adapter to be kept while the domain object
        ///     is replaced.
        /// </summary>
        public void ReplacePoco(object obj) {
            Object = obj;
        }

        public string ValidToPersist() {
            var objectSpec = Spec as IObjectSpec;
            Trace.Assert(objectSpec != null);

            IAssociationSpec[] properties = objectSpec.Properties;
            foreach (IAssociationSpec property in properties) {
                INakedObjectAdapter referencedObjectAdapter = property.GetNakedObject(this);
                if (property.IsUsable(this).IsAllowed && property.IsVisible(this)) {
                    if (property.IsMandatory && property.IsEmpty(this)) {
                        return string.Format(Resources.NakedObjects.PropertyMandatory, objectSpec.ShortName, property.Name);
                    }

                    var associationSpec = property as IOneToOneAssociationSpec;
                    if (associationSpec != null) {
                        IConsent valid = associationSpec.IsAssociationValid(this, referencedObjectAdapter);
                        if (valid.IsVetoed) {
                            return string.Format(Resources.NakedObjects.PropertyInvalid, objectSpec.ShortName, associationSpec.Name, valid.Reason);
                        }
                    }
                }

                if (property is IOneToOneAssociationSpec) {
                    if (referencedObjectAdapter != null && referencedObjectAdapter.ResolveState.IsTransient()) {
                        string referencedObjectMessage = referencedObjectAdapter.ValidToPersist();
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
            Assert.AssertTrue("New Oid must be transient", newOid.IsTransient);
            Oid = newOid;
        }

        public void CheckLock(IVersion otherVersion) {
            if (Version != null && Version.IsDifferent(otherVersion)) {
                throw new ConcurrencyException(this);
            }
        }

        public IOid Oid { get; private set; }

        public void LoadAnyComplexTypes() {
            if (Spec is IServiceSpec ||
                Spec.IsViewModel ||
                Spec.ContainsFacet(typeof(IComplexTypeFacet))) {
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
            int size = CanCount() ? facet.AsEnumerable(this, nakedObjectManager).Count() : CollectionUtils.IncompleteCollection;
            var elementSpecification = TypeOfFacet == null ? null : metamodel.GetSpecification(TypeOfFacet.GetValueSpec(this, metamodel.Metamodel));
            return CollectionUtils.CollectionTitleString(elementSpecification, size);
        }

        private bool CanCount() {
            return !Spec.ContainsFacet<INotCountedFacet>();
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

            str.AppendAsHex("poco-hash", Object.GetHashCode());
            return str.ToString();
        }

        private bool ShouldSetVersion(IVersion newVersion) {
            return newVersion.IsDifferent(Version);
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

            str.Append("version", Version == null ? null : Version.AsSequence());
        }

        private void CallCallback<T>() where T : ICallbackFacet {
            Spec.GetFacet<T>().Invoke(this, session, lifecycleManager, metamodel);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}