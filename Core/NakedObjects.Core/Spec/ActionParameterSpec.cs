// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Interactions;
using NakedObjects.Core.Reflect;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Spec {
    public abstract class ActionParameterSpec : IActionParameterSpec {
        private readonly IActionParameterSpecImmutable actionParameterSpecImmutable;
        private readonly INakedObjectManager manager;
        private readonly IMetamodelManager metamodel;
        private readonly IActionSpec parentAction;
        private readonly IObjectPersistor persistor;

        private readonly ISession session;

        // cache 
        private bool checkedForElementSpec;
        private Tuple<string, IObjectSpec>[] choicesParameters;
        private string description;
        private IObjectSpec elementSpec;
        private bool? isAutoCompleteEnabled;
        private bool? isChoicesEnabled;
        private bool? isMandatory;
        private bool? isMultipleChoicesEnabled;
        private bool? isNullable;
        private string name;
        private IObjectSpec spec;

        protected internal ActionParameterSpec(IMetamodelManager metamodel, int number, IActionSpec actionSpec, IActionParameterSpecImmutable actionParameterSpecImmutable, INakedObjectManager manager, ISession session, IObjectPersistor persistor) {
            Assert.AssertNotNull(metamodel);
            Assert.AssertNotNull(actionSpec);
            Assert.AssertNotNull(actionParameterSpecImmutable);
            Assert.AssertNotNull(manager);
            Assert.AssertNotNull(session);
            Assert.AssertNotNull(persistor);

            this.metamodel = metamodel;
            Number = number;
            parentAction = actionSpec;
            this.actionParameterSpecImmutable = actionParameterSpecImmutable;
            this.manager = manager;
            this.session = session;
            this.persistor = persistor;
        }

        public virtual IObjectSpec ElementSpec {
            get {
                if (!checkedForElementSpec) {
                    var facet = GetFacet<IElementTypeFacet>();
                    var es = facet != null ? facet.ValueSpec : null;
                    elementSpec = es == null ? null : metamodel.GetSpecification(es);
                    checkedForElementSpec = true;
                }

                return elementSpec;
            }
        }

        #region IActionParameterSpec Members

        public bool IsAutoCompleteEnabled {
            get {
                isAutoCompleteEnabled ??= ContainsFacet<IAutoCompleteFacet>();

                return isAutoCompleteEnabled.Value;
            }
        }

        public bool IsChoicesEnabled {
            get {
                isChoicesEnabled ??= !IsMultipleChoicesEnabled && actionParameterSpecImmutable.IsChoicesEnabled;

                return isChoicesEnabled.Value;
            }
        }

        public bool IsMultipleChoicesEnabled {
            get {
                isMultipleChoicesEnabled ??= Spec.IsCollectionOfBoundedSet(ElementSpec) ||
                                             Spec.IsCollectionOfEnum(ElementSpec) ||
                                             actionParameterSpecImmutable.IsMultipleChoicesEnabled;

                return isMultipleChoicesEnabled.Value;
            }
        }

        public virtual int Number { get; }

        public virtual IActionSpec Action => parentAction;

        public virtual IObjectSpec Spec => spec ??= metamodel.GetSpecification(actionParameterSpecImmutable.Specification);

        public string Name => name ??= GetFacet<INamedFacet>().NaturalName;

        public virtual string Description => description ??= GetFacet<IDescribedAsFacet>().Value ?? "";

        public virtual bool IsMandatory {
            get {
                isMandatory ??= GetFacet<IMandatoryFacet>().IsMandatory;

                return isMandatory.Value;
            }
        }

        public virtual Type[] FacetTypes => actionParameterSpecImmutable.FacetTypes;

        public virtual IIdentifier Identifier => parentAction.Identifier;

        public virtual bool ContainsFacet(Type facetType) => actionParameterSpecImmutable.ContainsFacet(facetType);

        public virtual bool ContainsFacet<T>() where T : IFacet => actionParameterSpecImmutable.ContainsFacet<T>();

        public virtual IFacet GetFacet(Type type) => actionParameterSpecImmutable.GetFacet(type);

        public virtual T GetFacet<T>() where T : IFacet => actionParameterSpecImmutable.GetFacet<T>();

        public virtual IEnumerable<IFacet> GetFacets() => actionParameterSpecImmutable.GetFacets();

        public IConsent IsValid(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter proposedValue) {
            if (proposedValue != null && !proposedValue.Spec.IsOfType(Spec)) {
                var msg = string.Format(Resources.NakedObjects.TypeMismatchError, Spec.SingularName);
                return GetConsent(msg);
            }

            var buf = new InteractionBuffer();
            IInteractionContext ic = InteractionContext.ModifyingPropParam(session, false, parentAction.RealTarget(nakedObjectAdapter), Identifier, proposedValue);
            InteractionUtils.IsValid(this, ic, buf);
            return InteractionUtils.IsValid(buf);
        }

        public virtual IConsent IsUsable(INakedObjectAdapter target) => Allow.Default;

        public bool IsNullable {
            get {
                isNullable ??= ContainsFacet(typeof(INullableFacet));

                return isNullable.Value;
            }
        }

        public Tuple<string, IObjectSpec>[] GetChoicesParameters() {
            if (choicesParameters == null) {
                var choicesFacet = GetFacet<IActionChoicesFacet>();
                choicesParameters = choicesFacet == null ? new Tuple<string, IObjectSpec>[] { } : choicesFacet.ParameterNamesAndTypes.Select(t => new Tuple<string, IObjectSpec>(t.Item1, metamodel.GetSpecification(t.Item2))).ToArray();
            }

            return choicesParameters;
        }

        public INakedObjectAdapter[] GetChoices(INakedObjectAdapter nakedObjectAdapter, IDictionary<string, INakedObjectAdapter> parameterNameValues) {
            var choicesFacet = GetFacet<IActionChoicesFacet>();
            var enumFacet = GetFacet<IEnumFacet>();

            if (choicesFacet != null) {
                var options = choicesFacet.GetChoices(parentAction.RealTarget(nakedObjectAdapter), parameterNameValues);
                if (enumFacet == null) {
                    return manager.GetCollectionOfAdaptedObjects(options).ToArray();
                }

                return manager.GetCollectionOfAdaptedObjects(enumFacet.GetChoices(parentAction.RealTarget(nakedObjectAdapter), options)).ToArray();
            }

            if (enumFacet != null) {
                return manager.GetCollectionOfAdaptedObjects(enumFacet.GetChoices(parentAction.RealTarget(nakedObjectAdapter))).ToArray();
            }

            if (Spec.IsBoundedSet()) {
                return manager.GetCollectionOfAdaptedObjects(persistor.Instances(Spec)).ToArray();
            }

            if (Spec.IsCollectionOfBoundedSet(ElementSpec) || Spec.IsCollectionOfEnum(ElementSpec)) {
                var elementEnumFacet = ElementSpec.GetFacet<IEnumFacet>();
                var domainObjects = elementEnumFacet != null ? (IEnumerable) elementEnumFacet.GetChoices(parentAction.RealTarget(nakedObjectAdapter)) : persistor.Instances(ElementSpec);
                return manager.GetCollectionOfAdaptedObjects(domainObjects).ToArray();
            }

            return null;
        }

        public INakedObjectAdapter[] GetCompletions(INakedObjectAdapter nakedObjectAdapter, string autoCompleteParm) {
            var autoCompleteFacet = GetFacet<IAutoCompleteFacet>();
            return autoCompleteFacet == null ? null : manager.GetCollectionOfAdaptedObjects(autoCompleteFacet.GetCompletions(parentAction.RealTarget(nakedObjectAdapter), autoCompleteParm)).ToArray();
        }

        public INakedObjectAdapter GetDefault(INakedObjectAdapter nakedObjectAdapter) => GetDefaultValueAndType(nakedObjectAdapter).Item1;

        public TypeOfDefaultValue GetDefaultType(INakedObjectAdapter nakedObjectAdapter) => GetDefaultValueAndType(nakedObjectAdapter).Item2;

        public string Id => Identifier.MemberParameterNames[Number];

        #endregion

        private Tuple<INakedObjectAdapter, TypeOfDefaultValue> GetDefaultValueAndType(INakedObjectAdapter nakedObjectAdapter) {
            if (parentAction.IsContributedMethod && nakedObjectAdapter != null) {
                var matchingParms = parentAction.Parameters.Where(p => nakedObjectAdapter.Spec.IsOfType(p.Spec)).ToArray();

                if (matchingParms.Any() && matchingParms.First() == this) {
                    return new Tuple<INakedObjectAdapter, TypeOfDefaultValue>(nakedObjectAdapter, TypeOfDefaultValue.Explicit);
                }
            }

            Tuple<object, TypeOfDefaultValue> defaultValue = null;

            // Check Facet on parm, then facet on type finally fall back on type; 

            var defaultsFacet = GetFacet<IActionDefaultsFacet>();
            if (defaultsFacet != null && !defaultsFacet.IsNoOp) {
                defaultValue = defaultsFacet.GetDefault(parentAction.RealTarget(nakedObjectAdapter));
            }

            if (defaultValue == null) {
                var defaultFacet = Spec.GetFacet<IDefaultedFacet>();
                if (defaultFacet != null && !defaultFacet.IsNoOp) {
                    defaultValue = new Tuple<object, TypeOfDefaultValue>(defaultFacet.Default, TypeOfDefaultValue.Implicit);
                }
            }

            if (defaultValue == null) {
                var rawValue = nakedObjectAdapter == null ? null : nakedObjectAdapter.Object.GetType().IsValueType ? (object) 0 : null;
                defaultValue = new Tuple<object, TypeOfDefaultValue>(rawValue, TypeOfDefaultValue.Implicit);
            }

            return new Tuple<INakedObjectAdapter, TypeOfDefaultValue>(manager.CreateAdapter(defaultValue.Item1, null, null), defaultValue.Item2);
        }

        private IConsent GetConsent(string message) => message == null ? (IConsent) Allow.Default : new Veto(message);
    }
}