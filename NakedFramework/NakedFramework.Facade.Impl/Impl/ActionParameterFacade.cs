// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Facade.Contexts;
using NakedFramework.Facade.Impl.Utility;
using NakedFramework.Facade.Interface;

namespace NakedFramework.Facade.Impl.Impl;

public class ActionParameterFacade : AbstractCommonFacade, IActionParameterFacade {
    private readonly INakedFramework framework;

    public ActionParameterFacade(IActionParameterSpec actionParameterSpec, IFrameworkFacade frameworkFacade, INakedFramework framework) : base(actionParameterSpec) {
        WrappedActionParameterSpec = actionParameterSpec ?? throw new NullReferenceException($"{nameof(actionParameterSpec)} is null");
        this.framework = framework ?? throw new NullReferenceException($"{nameof(framework)} is null");
        FrameworkFacade = frameworkFacade ?? throw new NullReferenceException($"{nameof(frameworkFacade)} is null");
    }

    public IActionParameterSpec WrappedActionParameterSpec { get; }

    public IFrameworkFacade FrameworkFacade { get; set; }

    private INakedObjectAdapter SafeGetValue(IActionParameterFacade parm, object rawValue) {
        try {
            return GetValue(parm, rawValue);
        }
        catch (Exception) {
            return null;
        }
    }

    private INakedObjectAdapter GetValue(IActionParameterFacade parm, object rawValue) {
        if (rawValue is null || (rawValue is string s && string.IsNullOrEmpty(s))) {
            return null;
        }

        if (parm.Specification.IsParseable) {
            return parm.WrappedSpec().Spec.GetFacet<IParseableFacet>().ParseTextEntry((string)rawValue, framework.NakedObjectManager);
        }

        if (parm.WrappedSpec() is IOneToManyActionParameterSpec collectionParm && collectionParm.ElementSpec.IsParseable) {
            if (rawValue is not string[] stringArray || !stringArray.Any()) {
                return null;
            }

            var eSpec = collectionParm.ElementSpec;

            var objectArray = stringArray.Select(i => i is null ? null : eSpec.GetFacet<IParseableFacet>().ParseTextEntry(i, framework.NakedObjectManager).Object).Where(o => o is not null).ToArray();

            if (!objectArray.Any()) {
                return null;
            }

            var typedArray = Array.CreateInstance(objectArray.First().GetType(), objectArray.Length);

            Array.Copy(objectArray, typedArray, typedArray.Length);

            return framework.GetNakedObject(typedArray);
        }

        return framework.GetNakedObject(rawValue);
    }

    private (string, ITypeFacade) WrapChoiceParm((string name, IObjectSpec spec) parm) => (parm.name, new TypeFacade(parm.spec, FrameworkFacade, framework));

    public override bool Equals(object obj) => obj is ActionParameterFacade apf && Equals(apf);

    private bool Equals(ActionParameterFacade other) => other is not null && (ReferenceEquals(this, other) || Equals(other.WrappedActionParameterSpec, WrappedActionParameterSpec));

    public override int GetHashCode() => WrappedActionParameterSpec.GetHashCode();

    #region IActionParameterFacade Members

    private NullCache<string> cachedMask;
    private int? cachedNumber;
    private TypeFacade cachedSpecification;
    private NullCache<TypeFacade> cachedElementType;
    private IActionFacade cachedAction;
    private Choices? cachedIsChoicesEnabled;
    private bool? cachedIsAutoCompleteEnabled;
    private (string, ITypeFacade)[] cachedChoicesParameters;
    private (IObjectFacade, TypeOfDefaultValue)? cachedDefaultAndType;
    private IConsentFacade cachedIsUsable;
    private bool? cachedIsInjected;
    private bool? cachedIsFindMenuEnabled;

    public string Grouping => "";

    public string Mask => (cachedMask ??= FacadeUtils.NullCache(WrappedActionParameterSpec.GetMask())).Value;

    public int Number => cachedNumber ??= WrappedActionParameterSpec.Number;

    public ITypeFacade Specification => cachedSpecification ??= new TypeFacade(WrappedActionParameterSpec.Spec, FrameworkFacade, framework);

    public ITypeFacade ElementType => (cachedElementType ??= FacadeUtils.NullCache(WrappedActionParameterSpec is IOneToManyActionParameterSpec pSpec ? new TypeFacade(pSpec.ElementSpec, FrameworkFacade, framework) : null)).Value;

    public IActionFacade Action => cachedAction ??= new ActionFacade(WrappedActionParameterSpec.Action, FrameworkFacade, framework);

    public string Id => WrappedActionParameterSpec.Id;

    public Choices IsChoicesEnabled(IObjectFacade objectFacade) =>
        cachedIsChoicesEnabled ??= WrappedActionParameterSpec.IsMultipleChoicesEnabled
            ? Choices.Multiple
            : WrappedActionParameterSpec.IsChoicesEnabled(objectFacade.WrappedAdapter())
                ? Choices.Single
                : Choices.NotEnabled;

    public bool IsAutoCompleteEnabled => cachedIsAutoCompleteEnabled ??= WrappedActionParameterSpec.IsAutoCompleteEnabled;

    public IObjectFacade[] GetChoices(IObjectFacade objectFacade, IDictionary<string, object> parameterNameValues) {
        var otherParms = parameterNameValues?.Select(kvp => new { kvp.Key, kvp.Value, parm = Action.Parameters.Single(p => p.Id == kvp.Key) });
        var pnv = otherParms?.ToDictionary(a => a.Key, a => SafeGetValue(a.parm, a.Value));
        return WrappedActionParameterSpec.GetChoices(((ObjectFacade)objectFacade)?.WrappedNakedObject, pnv).Select(no => ObjectFacade.Wrap(no, FrameworkFacade, framework)).Cast<IObjectFacade>().ToArray();
    }

    public (string, ITypeFacade)[] GetChoicesParameters() => cachedChoicesParameters ??= WrappedActionParameterSpec.GetChoicesParameters().Select(WrapChoiceParm).ToArray();

    public string GetMaskedValue(IObjectFacade objectFacade) => WrappedActionParameterSpec.GetMaskedValue(objectFacade, framework);

    public (IObjectFacade, string)[] GetChoicesAndTitles(IObjectFacade objectFacade, IDictionary<string, object> parameterNameValues) =>
        GetChoices(objectFacade, parameterNameValues).Select(c => (c, c.TitleString)).ToArray();

    public IObjectFacade[] GetCompletions(IObjectFacade objectFacade, string autoCompleteParm) => WrappedActionParameterSpec.GetCompletions(((ObjectFacade)objectFacade).WrappedNakedObject, autoCompleteParm).Select(no => ObjectFacade.Wrap(no, FrameworkFacade, framework)).Cast<IObjectFacade>().ToArray();

    private (IObjectFacade value, TypeOfDefaultValue type) GetDefaultAndType(IObjectFacade objectFacade) {
        (IObjectFacade value, TypeOfDefaultValue type) GetDefaultTuple() {
            var (defaultValue, defaultType) = WrappedActionParameterSpec.GetDefaultValueAndType(((ObjectFacade)objectFacade)?.WrappedNakedObject);
            return (ObjectFacade.Wrap(defaultValue, FrameworkFacade, framework), defaultType);
        }

        return cachedDefaultAndType ??= GetDefaultTuple();
    }

    public bool DefaultTypeIsExplicit(IObjectFacade objectFacade) => GetDefaultAndType(objectFacade).type is TypeOfDefaultValue.Explicit;

    public IObjectFacade GetDefault(IObjectFacade objectFacade) => GetDefaultAndType(objectFacade).value;

    public IConsentFacade IsUsable() => cachedIsUsable ??= new ConsentFacade(WrappedActionParameterSpec.IsUsable(null));

    public bool IsInjected => cachedIsInjected ??= WrappedActionParameterSpec.IsInjected;

    public bool IsFindMenuEnabled => cachedIsFindMenuEnabled ??= WrappedActionParameterSpec is IOneToOneActionParameterSpec { IsFindMenuEnabled: true };

    #endregion
}