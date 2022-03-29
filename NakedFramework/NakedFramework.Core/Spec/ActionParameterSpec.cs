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
using NakedFramework.Architecture.Interactions;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Core.Interactions;
using NakedFramework.Core.Reflect;
using NakedFramework.Core.Util;

namespace NakedFramework.Core.Spec;

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
    private bool? isInjected;
    private bool? isMandatory;
    private bool? isMultipleChoicesEnabled;
    private bool? isNullable;
    private string name;
    private IObjectSpec spec;

    protected internal ActionParameterSpec(int number, IActionSpec actionSpec, IActionParameterSpecImmutable actionParameterSpecImmutable, INakedFramework framework) {
        Number = number;
        Framework = framework;
        parentAction = actionSpec ?? throw new InitialisationException($"{nameof(actionSpec)} is null");
        this.actionParameterSpecImmutable = actionParameterSpecImmutable ?? throw new InitialisationException($"{nameof(actionParameterSpecImmutable)} is null");
    }

    public virtual IObjectSpec ElementSpec {
        get {
            if (!checkedForElementSpec) {
                var facet = GetFacet<IElementTypeFacet>();
                var es = facet?.GetElementSpec(Framework.MetamodelManager.Metamodel);
                elementSpec = es == null ? null : Framework.MetamodelManager.GetSpecification(es);
                checkedForElementSpec = true;
            }

            return elementSpec;
        }
    }

    public (INakedObjectAdapter value, TypeOfDefaultValue type) GetDefaultValueAndType(INakedObjectAdapter nakedObjectAdapter) {
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

    private static IConsent GetConsent(string message) => message is null ? Allow.Default : new Veto(message);

    #region IActionParameterSpec Members

    public bool IsAutoCompleteEnabled {
        get {
            isAutoCompleteEnabled ??= ContainsFacet<IAutoCompleteFacet>();
            return isAutoCompleteEnabled.Value;
        }
    }

    public bool IsChoicesEnabled(INakedObjectAdapter adapter) {
        isChoicesEnabled ??= !IsMultipleChoicesEnabled && actionParameterSpecImmutable.IsChoicesEnabled(adapter, Framework);
        return isChoicesEnabled.Value;
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
    protected INakedFramework Framework { get; }

    public virtual IActionSpec Action => parentAction;

    public virtual IObjectSpec Spec => spec ??= Framework.MetamodelManager.GetSpecification(actionParameterSpecImmutable.Specification);

    public string Name(INakedObjectAdapter nakedObjectAdapter) => name ??= GetFacet<IMemberNamedFacet>().FriendlyName(nakedObjectAdapter, Framework);

    public virtual string Description(INakedObjectAdapter nakedObjectAdapter) => description ??= GetFacet<IDescribedAsFacet>().Description(nakedObjectAdapter, Framework) ?? "";

    public virtual bool IsMandatory {
        get {
            isMandatory ??= GetFacet<IMandatoryFacet>().IsMandatory;
            return isMandatory.Value;
        }
    }

    public virtual bool IsInjected {
        get {
            isInjected ??= GetFacet<IInjectedFacet>() is not null;
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
        if (proposedValue is not null && !proposedValue.Spec.IsOfType(Spec)) {
            var msg = string.Format(NakedObjects.Resources.NakedObjects.TypeMismatchError, Spec.SingularName);
            return GetConsent(msg);
        }

        var buf = new InteractionBuffer();
        IInteractionContext ic = InteractionContext.ModifyingPropParam(Framework, false, parentAction.RealTarget(nakedObjectAdapter), Identifier, proposedValue);
        InteractionUtils.IsValid(this, ic, buf);
        return InteractionUtils.IsValid(buf);
    }

    private string GetDisabledMessage() => GetFacet<IDisabledFacet>() is { } df ? df.DisabledReason(null) : null;

    public virtual IConsent IsUsable(INakedObjectAdapter target) => GetConsent(GetDisabledMessage());

    public bool IsNullable {
        get {
            isNullable ??= ContainsFacet(typeof(INullableFacet));
            return isNullable.Value;
        }
    }

    public (string, IObjectSpec)[] GetChoicesParameters() {
        if (choicesParameters is null) {
            var choicesFacet = GetFacet<IActionChoicesFacet>();
            choicesParameters = choicesFacet is null
                ? Array.Empty<(string, IObjectSpec)>()
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

        if (choicesFacet is not null) {
            var options = choicesFacet.GetChoices(parentAction.RealTarget(nakedObjectAdapter), parameterNameValues, Framework);

            if (enumFacet is not null) {
                options = enumFacet.GetChoices(parentAction.RealTarget(nakedObjectAdapter), options);
            }

            return manager.GetCollectionOfAdaptedObjects(options).ToArray();
        }

        if (enumFacet is not null) {
            return manager.GetCollectionOfAdaptedObjects(enumFacet.GetChoices(parentAction.RealTarget(nakedObjectAdapter))).ToArray();
        }

        if (Spec.IsBoundedSet()) {
            return manager.GetCollectionOfAdaptedObjects(persistor.Instances(Spec)).ToArray();
        }

        if (Spec.IsCollectionOfBoundedSet(ElementSpec) || Spec.IsCollectionOfEnum(ElementSpec)) {
            var elementEnumFacet = ElementSpec.GetFacet<IEnumFacet>();
            var domainObjects = elementEnumFacet != null ? (IEnumerable)elementEnumFacet.GetChoices(parentAction.RealTarget(nakedObjectAdapter)) : persistor.Instances(ElementSpec);
            return manager.GetCollectionOfAdaptedObjects(domainObjects).ToArray();
        }

        return null;
    }

    public INakedObjectAdapter[] GetCompletions(INakedObjectAdapter nakedObjectAdapter, string autoCompleteParm) {
        var autoCompleteFacet = GetFacet<IAutoCompleteFacet>();
        return autoCompleteFacet is null ? null : Framework.NakedObjectManager.GetCollectionOfAdaptedObjects(autoCompleteFacet.GetCompletions(parentAction.RealTarget(nakedObjectAdapter), autoCompleteParm, Framework)).ToArray();
    }

    public INakedObjectAdapter GetDefault(INakedObjectAdapter nakedObjectAdapter) => GetDefaultValueAndType(nakedObjectAdapter).value;

    public TypeOfDefaultValue GetDefaultType(INakedObjectAdapter nakedObjectAdapter) => GetDefaultValueAndType(nakedObjectAdapter).type;

    public string Id => Identifier.MemberParameterNames[Number];

    #endregion
}