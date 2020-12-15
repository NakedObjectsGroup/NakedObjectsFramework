// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Reflect;
using NakedObjects.Core.Resolve;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Spec {
    public abstract class AssociationSpecAbstract : MemberSpecAbstract, IAssociationSpec {
        protected AssociationSpecAbstract(IAssociationSpecImmutable association,  INakedObjectsFramework framework)
            : base(association.Identifier.MemberName, association, framework) {
            Manager = framework.NakedObjectManager ?? throw new InitialisationException($"{nameof(framework.NakedObjectManager)} is null");
            ReturnSpec = MetamodelManager.GetSpecification(association.ReturnSpec);
        }

        public virtual bool IsAutoCompleteEnabled => ContainsFacet(typeof(IAutoCompleteFacet));

        public INakedObjectManager Manager { get; }

        #region IAssociationSpec Members

        /// <summary>
        ///     Return the specification of the object (or objects) that this field holds. For a value are one-to-one
        ///     reference this will be type that the accessor returns. For a collection it will be the type of element,
        ///     not the type of collection.
        /// </summary>
        public override IObjectSpec ReturnSpec { get; }

        /// <summary>
        ///     Returns true if this field is persisted, and not calculated from other data in the object or used transiently.
        /// </summary>
        public virtual bool IsPersisted => !ContainsFacet(typeof(INotPersistedFacet));

        public virtual bool IsReadOnly => !ContainsFacet<IPropertySetterFacet>();

        public abstract bool IsMandatory { get; }
        public abstract INakedObjectAdapter GetNakedObject(INakedObjectAdapter fromObjectAdapter);
        public abstract bool IsEmpty(INakedObjectAdapter inObjectAdapter);
        public abstract bool IsInline { get; }
        public abstract INakedObjectAdapter GetDefault(INakedObjectAdapter nakedObjectAdapter);
        public abstract TypeOfDefaultValue GetDefaultType(INakedObjectAdapter nakedObjectAdapter);
        public abstract void ToDefault(INakedObjectAdapter nakedObjectAdapter);

        public override IConsent IsUsable(INakedObjectAdapter target) {
            var isPersistent = target.ResolveState.IsPersistent();
            var disabledConsent = IsUsableDeclaratively(isPersistent);
            if (disabledConsent != null) {
                return disabledConsent;
            }

            var viewModelFacet = target.Spec.GetFacet<IViewModelFacet>();

            if (viewModelFacet != null) {
                // all fields on a non-editable view model are disabled
                if (!viewModelFacet.IsEditView(target, Session, Persistor)) {
                    return new Veto(Resources.NakedObjects.FieldDisabled);
                }
            }

            var immutableFacet = GetFacet<IImmutableFacet>();
            if (immutableFacet != null) {
                var when = immutableFacet.Value;
                switch (when) {
                    case WhenTo.UntilPersisted when !isPersistent:
                        return new Veto(Resources.NakedObjects.FieldDisabledUntil);
                    case WhenTo.OncePersisted when isPersistent:
                        return new Veto(Resources.NakedObjects.FieldDisabledOnce);
                }

                var tgtSpec = target.Spec;
                if (tgtSpec.IsAlwaysImmutable() || tgtSpec.IsImmutableOncePersisted() && isPersistent) {
                    return new Veto(Resources.NakedObjects.FieldDisabled);
                }
            }

            var f = GetFacet<IDisableForContextFacet>();
            var reason = f?.DisabledReason(target, Session, Persistor);

            if (reason == null) {
                var fs = GetFacet<IDisableForSessionFacet>();
                reason = fs == null ? null : fs.DisabledReason(Session, target, LifecycleManager, MetamodelManager);
            }

            return GetConsent(reason);
        }

        #endregion

        public abstract INakedObjectAdapter[] GetCompletions(INakedObjectAdapter nakedObjectAdapter, string autoCompleteParm);

        private IConsent IsUsableDeclaratively(bool isPersistent) {
            var facet = GetFacet<IDisabledFacet>();
            if (facet != null) {
                var isProtected = facet.Value;
                switch (isProtected) {
                    case WhenTo.Always:
                        return new Veto(Resources.NakedObjects.FieldNotEditable);
                    case WhenTo.OncePersisted when isPersistent:
                        return new Veto(Resources.NakedObjects.FieldNotEditableNow);
                    case WhenTo.UntilPersisted when !isPersistent:
                        return new Veto(Resources.NakedObjects.FieldNotEditableUntil);
                }
            }

            return null;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}