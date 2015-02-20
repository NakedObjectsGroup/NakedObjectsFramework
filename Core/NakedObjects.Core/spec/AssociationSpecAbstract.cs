// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
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
    internal abstract class AssociationSpecAbstract : MemberSpecAbstract, IAssociationSpec {
        private readonly INakedObjectManager manager;
        private readonly IObjectSpec returnSpec;

        protected AssociationSpecAbstract(IMetamodelManager metamodel, IAssociationSpecImmutable association, ISession session, ILifecycleManager lifecycleManager, INakedObjectManager manager)
            : base(association.Identifier.MemberName, association, session, lifecycleManager, metamodel) {
            Assert.AssertNotNull(manager);

            this.manager = manager;
            returnSpec = MetamodelManager.GetSpecification(association.ReturnSpec);
        }

        public virtual bool IsChoicesEnabled {
            get { return ContainsFacet(typeof (IPropertyChoicesFacet)); }
        }

        public virtual bool IsAutoCompleteEnabled {
            get { return ContainsFacet(typeof (IAutoCompleteFacet)); }
        }

        public INakedObjectManager Manager {
            get { return manager; }
        }

        public virtual bool IsASet {
            get { return false; }
        }

        #region IAssociationSpec Members

        /// <summary>
        ///     Return the specification of the object (or objects) that this field holds. For a value are one-to-one
        ///     reference this will be type that the accessor returns. For a collection it will be the type of element,
        ///     not the type of collection.
        /// </summary>
        public override IObjectSpec ReturnSpec {
            get { return returnSpec; }
        }

        /// <summary>
        ///     Returns true if this field is persisted, and not calculated from other data in the object or used transiently.
        /// </summary>
        public virtual bool IsPersisted {
            get { return !ContainsFacet(typeof (INotPersistedFacet)); }
        }

        public virtual bool IsReadOnly {
            get { return !ContainsFacet<IPropertySetterFacet>(); }
        }

        public abstract bool IsMandatory { get; }
        public abstract INakedObject GetNakedObject(INakedObject fromObject);
        public abstract bool IsEmpty(INakedObject inObject);
        public abstract bool IsInline { get; }
        public abstract INakedObject GetDefault(INakedObject nakedObject);
        public abstract TypeOfDefaultValue GetDefaultType(INakedObject nakedObject);
        public abstract void ToDefault(INakedObject nakedObject);

        public override IConsent IsUsable(INakedObject target) {
            bool isPersistent = target.ResolveState.IsPersistent();
            IConsent disabledConsent = IsUsableDeclaratively(isPersistent);
            if (disabledConsent != null) {
                return disabledConsent;
            }
            var immutableFacet = GetFacet<IImmutableFacet>();
            if (immutableFacet != null) {
                WhenTo when = immutableFacet.Value;
                if (when == WhenTo.UntilPersisted && !isPersistent) {
                    return new Veto(Resources.NakedObjects.FieldDisabledUntil);
                }
                if (when == WhenTo.OncePersisted && isPersistent) {
                    return new Veto(Resources.NakedObjects.FieldDisabledOnce);
                }
                ITypeSpec tgtSpec = target.Spec;
                if (tgtSpec.IsAlwaysImmutable() || (tgtSpec.IsImmutableOncePersisted() && isPersistent)) {
                    return new Veto(Resources.NakedObjects.FieldDisabled);
                }
            }
            var f = GetFacet<IDisableForContextFacet>();
            string reason = f == null ? null : f.DisabledReason(target);

            if (reason == null) {
                var fs = GetFacet<IDisableForSessionFacet>();
                reason = fs == null ? null : fs.DisabledReason(Session, target, LifecycleManager, MetamodelManager);
            }

            return GetConsent(reason);
        }

        #endregion

        public abstract INakedObject[] GetChoices(INakedObject nakedObject, IDictionary<string, INakedObject> parameterNameValues);
        public abstract Tuple<string, IObjectSpec>[] GetChoicesParameters();
        public abstract INakedObject[] GetCompletions(INakedObject nakedObject, string autoCompleteParm);

        private IConsent IsUsableDeclaratively(bool isPersistent) {
            var facet = GetFacet<IDisabledFacet>();
            if (facet != null) {
                WhenTo isProtected = facet.Value;
                if (isProtected == WhenTo.Always) {
                    return new Veto(Resources.NakedObjects.FieldNotEditable);
                }
                if (isProtected == WhenTo.OncePersisted && isPersistent) {
                    return new Veto(Resources.NakedObjects.FieldNotEditableNow);
                }
                if (isProtected == WhenTo.UntilPersisted && !isPersistent) {
                    return new Veto(Resources.NakedObjects.FieldNotEditableUntil);
                }
            }

            return null;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}