// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.AutoComplete;
using NakedObjects.Architecture.Facets.Disable;
using NakedObjects.Architecture.Facets.Objects.Immutable;
using NakedObjects.Architecture.Facets.Propcoll.NotPersisted;
using NakedObjects.Architecture.Facets.Properties.Choices;
using NakedObjects.Architecture.Facets.Properties.Modify;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Reflect {
    public abstract class NakedObjectAssociationAbstract : NakedObjectMemberAbstract, INakedObjectAssociation {
        private readonly INakedObjectSpecification specification;

        protected NakedObjectAssociationAbstract(string fieldId, INakedObjectSpecification specification, IFacetHolder facetHolder)
            : base(fieldId, facetHolder) {
            if (specification == null) {
                throw new ArgumentException(string.Format(Resources.NakedObjects.MissingFieldType, fieldId));
            }
            this.specification = specification;
        }

        public virtual bool IsChoicesEnabled {
            get { return ContainsFacet(typeof (IPropertyChoicesFacet)); }
        }


        public virtual bool IsAutoCompleteEnabled {
            get { return ContainsFacet(typeof (IAutoCompleteFacet)); }
        }

        #region INakedObjectAssociation Members

        /// <summary>
        ///     Return the specification of the object (or objects) that this field holds. For a value are one-to-one
        ///     reference this will be type that the accessor returns. For a collection it will be the type of element,
        ///     not the type of collection.
        /// </summary>
        public override INakedObjectSpecification Specification {
            get { return specification; }
        }

        /// <summary>
        ///     Returns true if this field is for a collection
        /// </summary>
        public virtual bool IsCollection {
            get { return false; }
        }

        public virtual bool IsASet {
            get { return false; }
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

        /// <summary>
        ///     Returns true if this field is for an object, not a collection
        /// </summary>
        public virtual bool IsObject {
            get { return false; }
        }

        public abstract bool IsMandatory { get; }

        public abstract INakedObject GetNakedObject(INakedObject fromObject, INakedObjectManager manager);

        public abstract bool IsEmpty(INakedObject inObject, INakedObjectManager manager, IObjectPersistor persistor);
        public abstract bool IsInline { get; }

        public abstract INakedObject GetDefault(INakedObject nakedObject, INakedObjectManager manager);
        public abstract TypeOfDefaultValue GetDefaultType(INakedObject nakedObject, INakedObjectManager manager);
        public abstract void ToDefault(INakedObject nakedObject, INakedObjectManager manager);

        public override IConsent IsUsable(ISession session, INakedObject target, ILifecycleManager persistor) {
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
                INakedObjectSpecification tgtSpec = target.Specification;
                if (tgtSpec.IsAlwaysImmutable() || (tgtSpec.IsImmutableOncePersisted() && isPersistent)) {
                    return new Veto(Resources.NakedObjects.FieldDisabled);
                }
            }
            var f = GetFacet<IDisableForContextFacet>();
            string reason = f == null ? null : f.DisabledReason(target);

            if (reason == null) {
                var fs = GetFacet<IDisableForSessionFacet>();
                reason = fs == null ? null : fs.DisabledReason(session, target, persistor);
            }

            return GetConsent(reason);
        }

        #endregion

        public abstract INakedObject[] GetChoices(INakedObject nakedObject, IDictionary<string, INakedObject> parameterNameValues, ILifecycleManager persistor);

        public abstract Tuple<string, INakedObjectSpecification>[] GetChoicesParameters();

        public abstract INakedObject[] GetCompletions(INakedObject nakedObject, string autoCompleteParm, ILifecycleManager persistor);

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