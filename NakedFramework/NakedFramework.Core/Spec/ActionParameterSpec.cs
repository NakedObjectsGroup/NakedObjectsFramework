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
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.interactions;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Interactions;
using NakedObjects.Core.Reflect;
using NakedObjects.Core.Util;

namespace NakedFramework.Core.Spec {
    public abstract class ActionParameterSpec : IActionParameterSpec {
        private readonly IActionParameterSpecImmutable actionParameterSpecImmutable;
     
        private readonly IActionSpec parentAction;
     

        // cache 
        private bool checkedForElementSpec;
        private (string, IObjectSpec)[] choicesParameters;
        private string description;
        private IObjectSpec elementSpec;
        private bool? isAutoCompleteEnabled;
        private bool? isChoicesEnabled;
        private bool? isMandatory;
        private bool? isInjected;
        private bool? isMultipleChoicesEnabled;
        private bool? isNullable;
        private string name;
        private IObjectSpec spec;

        protected internal ActionParameterSpec(int number, IActionSpec actionSpec, IActionParameterSpecImmutable actionParameterSpecImmutable, INakedObjectsFramework framework) {
            Number = number;
            Framework = framework;
            parentAction = actionSpec ?? throw new InitialisationException($"{nameof(actionSpec)} is null");
            this.actionParameterSpecImmutable = actionParameterSpecImmutable ?? throw new InitialisationException($"{nameof(actionParameterSpecImmutable)} is null");
        }

        public virtual IObjectSpec ElementSpec {
            get {
                if (!checkedForElementSpec) {
                    var facet = GetFacet<IElementTypeFacet>();
                    var es = facet != null ? facet.ValueSpec : null;
                    elementSpec = es == null ? null : Framework.MetamodelManager.GetSpecification(es);
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
        protected INakedObjectsFramework Framework { get; }

        public virtual IActionSpec Action => parentAction;

        public virtual IObjectSpec Spec => spec ??=  Framework.MetamodelManager.GetSpecification(actionParameterSpecImmutable.Specification);

        public string Name => name ??= GetFacet<INamedFacet>().NaturalName;

        public virtual string Description => description ??= GetFacet<IDescribedAsFacet>().Value ?? "";

        public virtual bool IsMandatory {
            get {
                isMandatory ??= GetFacet<IMandatoryFacet>().IsMandatory;
                return isMandatory.Value;
            }
        }

        public virtual bool IsInjected
        {
            get {
                isInjected ??= GetFacet<IInjectedFacet>() != null;
                return isInjected.Value;
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
                var msg = string.Format(NakedObjects.Resources.NakedObjects.TypeMismatchError, Spec.SingularName);
                return GetConsent(msg);
            }

            var buf = new InteractionBuffer();
            IInteractionContext ic = InteractionContext.ModifyingPropParam(Framework, false, parentAction.RealTarget(nakedObjectAdapter), Identifier, proposedValue);
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

        public (string, IObjectSpec)[] GetChoicesParameters() {
            if (choicesParameters == null) {
                var choicesFacet = GetFacet<IActionChoicesFacet>();
                choicesParameters = choicesFacet == null
                    ? new (string, IObjectSpec)[] { }
                    : choicesFacet.ParameterNamesAndTypes.Select(t => {
                        var (pName, pSpec) = t;
                        return (pName, Framework.MetamodelManager.GetSpecification(pSpec));
                    }).ToArray();
            }

            return choicesParameters;
        }

        public INakedObjectAdapter[] GetChoices(INakedObjectAdapter nakedObjectAdapter, IDictionary<string, INakedObjectAdapter> parameterNameValues) {
            var choicesFacet = GetFacet<IActionChoicesFacet>();
            var enumFacet = GetFacet<IEnumFacet>();
            var manager = Framework.NakedObjectManager;
            var persistor = Framework.Persistor;

            if (choicesFacet != null) {
                var options = choicesFacet.GetChoices(parentAction.RealTarget(nakedObjectAdapter), parameterNameValues, Framework);
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
            return autoCompleteFacet == null ? null : Framework.NakedObjectManager.GetCollectionOfAdaptedObjects(autoCompleteFacet.GetCompletions(parentAction.RealTarget(nakedObjectAdapter), autoCompleteParm, Framework)).ToArray();
        }

        public INakedObjectAdapter GetDefault(INakedObjectAdapter nakedObjectAdapter) => GetDefaultValueAndType(nakedObjectAdapter).value;

        public TypeOfDefaultValue GetDefaultType(INakedObjectAdapter nakedObjectAdapter) => GetDefaultValueAndType(nakedObjectAdapter).type;

        public string Id => Identifier.MemberParameterNames[Number];

        #endregion

        private (INakedObjectAdapter value, TypeOfDefaultValue type) GetDefaultValueAndType(INakedObjectAdapter nakedObjectAdapter) {
            if (parentAction.IsContributedMethod && nakedObjectAdapter != null) {
                var matchingParms = parentAction.Parameters.Where(p => nakedObjectAdapter.Spec.IsOfType(p.Spec)).ToArray();

                if (matchingParms.Any() && matchingParms.First() == this) {
                    return (nakedObjectAdapter, TypeOfDefaultValue.Explicit);
                }
            }

            var facet = this.GetOpFacet<IActionDefaultsFacet>() ?? Spec.GetOpFacet<IDefaultedFacet>();

            var (domainObject, typeOfDefaultValue) = facet switch {
                IActionDefaultsFacet adf => adf.GetDefault(parentAction.RealTarget(nakedObjectAdapter), Framework),
                IDefaultedFacet df => (df.Default, TypeOfDefaultValue.Implicit),
                _ when nakedObjectAdapter == null => (null, TypeOfDefaultValue.Implicit),
                _ when nakedObjectAdapter.Object.GetType().IsValueType => (0, TypeOfDefaultValue.Implicit),
                _ => (null, TypeOfDefaultValue.Implicit)
            };

            return (Framework.NakedObjectManager.CreateAdapter(domainObject, null, null), typeOfDefaultValue);
        }

        private static IConsent GetConsent(string message) => message == null ? (IConsent) Allow.Default : new Veto(message);
    }
}