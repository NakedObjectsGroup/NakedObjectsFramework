// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Facade.Contexts;
using NakedFramework.Facade.Impl.Utility;
using NakedFramework.Facade.Interface;

namespace NakedFramework.Facade.Impl.Impl {
    public class ActionParameterFacade : IActionParameterFacade {
        private readonly INakedObjectsFramework framework;

        public ActionParameterFacade(IActionParameterSpec nakedObjectActionParameter, IFrameworkFacade frameworkFacade, INakedObjectsFramework framework) {
            WrappedSpec = nakedObjectActionParameter ?? throw new NullReferenceException($"{nameof(nakedObjectActionParameter)} is null");
            this.framework = framework ?? throw new NullReferenceException($"{nameof(framework)} is null");
            FrameworkFacade = frameworkFacade ?? throw new NullReferenceException($"{nameof(frameworkFacade)} is null");
        }

        public IActionParameterSpec WrappedSpec { get; }

        private INakedObjectAdapter SafeGetValue(IActionParameterFacade parm, object rawValue) {
            try {
                return GetValue(parm, rawValue);
            }
            catch (Exception) {
                return null;
            }
        }

        private INakedObjectAdapter GetValue(IActionParameterFacade parm, object rawValue) {
            if (rawValue is null || rawValue is string s && string.IsNullOrEmpty(s)) {
                return null;
            }

            if (parm.Specification.IsParseable) {
                return parm.WrappedSpec().Spec.GetFacet<IParseableFacet>().ParseTextEntry((string) rawValue, framework.NakedObjectManager);
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

        private bool Equals(ActionParameterFacade other) => other is not null && (ReferenceEquals(this, other) || Equals(other.WrappedSpec, WrappedSpec));

        public override int GetHashCode() => WrappedSpec != null ? WrappedSpec.GetHashCode() : 0;

        #region IActionParameterFacade Members

        public string Name => WrappedSpec.Name;

        public string Description => WrappedSpec.Description;

        public bool IsMandatory => WrappedSpec.IsMandatory;

        public int? MaxLength => WrappedSpec.GetMaxLength();

        public string Grouping => "";

        public DataType? DataType =>
            WrappedSpec.GetFacet<IDataTypeFacet>()?.DataType() ?? WrappedSpec.GetFacet<IPasswordFacet>()?.DataType;

        public bool IsDateOnly => WrappedSpec.ContainsFacet<IDateOnlyFacet>();

        public string Pattern => WrappedSpec.GetPattern();

        public string Mask => WrappedSpec.GetMask();

        public int AutoCompleteMinLength => WrappedSpec.GetAutoCompleteMinLength();

        public int Number => WrappedSpec.Number;

        public ITypeFacade Specification => new TypeFacade(WrappedSpec.Spec, FrameworkFacade, framework);

        public ITypeFacade ElementType {
            get {
                var parm = WrappedSpec as IOneToManyActionParameterSpec;
                var elementSpec = parm?.ElementSpec;
                return elementSpec == null ? null : new TypeFacade(elementSpec, FrameworkFacade, framework);
            }
        }

        public IActionFacade Action => new ActionFacade(WrappedSpec.Action, FrameworkFacade, framework);

        public string Id => WrappedSpec.Id;

        public Choices IsChoicesEnabled =>
            WrappedSpec.IsMultipleChoicesEnabled
                ? Choices.Multiple
                : WrappedSpec.IsChoicesEnabled
                    ? Choices.Single
                    : Choices.NotEnabled;

        public bool IsAutoCompleteEnabled => WrappedSpec.IsAutoCompleteEnabled;

        public IObjectFacade[] GetChoices(IObjectFacade objectFacade, IDictionary<string, object> parameterNameValues) {
            var otherParms = parameterNameValues?.Select(kvp => new {kvp.Key, kvp.Value, parm = Action.Parameters.Single(p => p.Id == kvp.Key)});

            var pnv = otherParms?.ToDictionary(a => a.Key, a => SafeGetValue(a.parm, a.Value));

            return WrappedSpec.GetChoices(((ObjectFacade) objectFacade)?.WrappedNakedObject, pnv).Select(no => ObjectFacade.Wrap(no, FrameworkFacade, framework)).Cast<IObjectFacade>().ToArray();
        }

        public (string, ITypeFacade)[] GetChoicesParameters() => WrappedSpec.GetChoicesParameters().Select(WrapChoiceParm).ToArray();

        public string GetMaskedValue(IObjectFacade objectFacade) => WrappedSpec.GetMaskedValue(objectFacade, framework);

        public (IObjectFacade, string)[] GetChoicesAndTitles(IObjectFacade objectFacade, IDictionary<string, object> parameterNameValues) =>
            GetChoices(objectFacade, parameterNameValues).Select(c => (c, c.TitleString)).ToArray();

        public IObjectFacade[] GetCompletions(IObjectFacade objectFacade, string autoCompleteParm) => WrappedSpec.GetCompletions(((ObjectFacade) objectFacade).WrappedNakedObject, autoCompleteParm).Select(no => ObjectFacade.Wrap(no, FrameworkFacade, framework)).Cast<IObjectFacade>().ToArray();

        public bool DefaultTypeIsExplicit(IObjectFacade objectFacade) => WrappedSpec.GetDefaultType(((ObjectFacade) objectFacade)?.WrappedNakedObject) == TypeOfDefaultValue.Explicit;

        public IObjectFacade GetDefault(IObjectFacade objectFacade) => ObjectFacade.Wrap(WrappedSpec.GetDefault(((ObjectFacade) objectFacade)?.WrappedNakedObject), FrameworkFacade, framework);
        public IConsentFacade IsUsable() =>  new ConsentFacade(WrappedSpec.IsUsable(null));

        public bool IsInjected => WrappedSpec.IsInjected;

        public IFrameworkFacade FrameworkFacade { get; set; }

        public bool IsFindMenuEnabled => WrappedSpec is IOneToOneActionParameterSpec {IsFindMenuEnabled: true};

        public (Regex, string)? RegEx => WrappedSpec.GetRegEx();

        public (IConvertible, IConvertible, bool)? Range => WrappedSpec.GetRange();

        public bool IsPassword => WrappedSpec.ContainsFacet<IPasswordFacet>();

        public bool IsNullable => WrappedSpec.ContainsFacet<INullableFacet>();

        public int Width => WrappedSpec.GetWidth();

        public string PresentationHint => WrappedSpec.GetPresentationHint();

        public int NumberOfLines => WrappedSpec.GetNumberOfLinesWithDefault();

        #endregion
    }
}