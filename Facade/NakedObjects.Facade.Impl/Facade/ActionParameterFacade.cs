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
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core;
using NakedObjects.Core.Reflect;
using NakedObjects.Facade.Contexts;
using NakedObjects.Facade.Impl.Utility;

namespace NakedObjects.Facade.Impl {
    public class ActionParameterFacade : IActionParameterFacade {
        private readonly INakedObjectsFramework framework;
        private readonly string overloadedUniqueId;

        public ActionParameterFacade(IActionParameterSpec nakedObjectActionParameter, IFrameworkFacade frameworkFacade, INakedObjectsFramework framework, string overloadedUniqueId) {
            FacadeUtils.AssertNotNull(nakedObjectActionParameter, "Action Parameter is null");
            FacadeUtils.AssertNotNull(framework, "framework is null");
            FacadeUtils.AssertNotNull(overloadedUniqueId, "overloadedUniqueId is null");
            FacadeUtils.AssertNotNull(frameworkFacade, "FrameworkFacade is null");

            WrappedSpec = nakedObjectActionParameter;
            this.framework = framework;
            this.overloadedUniqueId = overloadedUniqueId;
            FrameworkFacade = frameworkFacade;
        }

        public IActionParameterSpec WrappedSpec { get; }

        #region IActionParameterFacade Members

        public string Name => WrappedSpec.Name;

        public string Description => WrappedSpec.Description;

        public bool IsMandatory => WrappedSpec.IsMandatory;

        public int? MaxLength => WrappedSpec.GetMaxLength();

        public DataType? DataType => WrappedSpec.GetFacet<IDataTypeFacet>()?.DataType();

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

        public IActionFacade Action => new ActionFacade(WrappedSpec.Action, FrameworkFacade, framework, overloadedUniqueId ?? "");

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

            return WrappedSpec.GetChoices(((ObjectFacade) objectFacade).WrappedNakedObject, pnv).Select(no => ObjectFacade.Wrap(no, FrameworkFacade, framework)).Cast<IObjectFacade>().ToArray();
        }

        public Tuple<string, ITypeFacade>[] GetChoicesParameters() => WrappedSpec.GetChoicesParameters().Select(WrapChoiceParm).ToArray();

        public string GetMaskedValue(IObjectFacade objectFacade) => WrappedSpec.GetMaskedValue(objectFacade, framework.NakedObjectManager);

        public IConsentFacade IsValid(IObjectFacade target, object value) {
            var t = ((ObjectFacade) target).WrappedNakedObject;

            IConsent consent;
            try {
                var v = GetValue(this, value);
                consent = WrappedSpec.IsValid(t, v);
            }
            catch (InvalidEntryException) {
                consent = new Veto(Resources.NakedObjects.InvalidEntry);
            }
            catch (Exception e) {
                consent = new Veto(e.Message);
            }

            return new ConsentFacade(consent);
        }

        public Tuple<IObjectFacade, string>[] GetChoicesAndTitles(IObjectFacade objectFacade, IDictionary<string, object> parameterNameValues) {
            var choices = GetChoices(objectFacade, parameterNameValues);
            return choices.Select(c => new Tuple<IObjectFacade, string>(c, c.TitleString)).ToArray();
        }

        public IObjectFacade[] GetCompletions(IObjectFacade objectFacade, string autoCompleteParm) => WrappedSpec.GetCompletions(((ObjectFacade) objectFacade).WrappedNakedObject, autoCompleteParm).Select(no => ObjectFacade.Wrap(no, FrameworkFacade, framework)).Cast<IObjectFacade>().ToArray();

        public bool DefaultTypeIsExplicit(IObjectFacade objectFacade) => WrappedSpec.GetDefaultType(((ObjectFacade) objectFacade).WrappedNakedObject) == TypeOfDefaultValue.Explicit;

        public IObjectFacade GetDefault(IObjectFacade objectFacade) => ObjectFacade.Wrap(WrappedSpec.GetDefault(((ObjectFacade) objectFacade).WrappedNakedObject), FrameworkFacade, framework);

        public IFrameworkFacade FrameworkFacade { get; set; }

        public bool IsFindMenuEnabled => WrappedSpec is IOneToOneActionParameterSpec ws && ws.IsFindMenuEnabled;

        public Tuple<Regex, string> RegEx => WrappedSpec.GetRegEx();

        public Tuple<IConvertible, IConvertible, bool> Range => WrappedSpec.GetRange();

        public bool IsAjax => !WrappedSpec.ContainsFacet<IAjaxFacet>();

        public bool IsPassword => WrappedSpec.ContainsFacet<IPasswordFacet>();

        public int TypicalLength => WrappedSpec.GetTypicalLength();

        public bool IsNullable => WrappedSpec.ContainsFacet<INullableFacet>();

        public int Width => WrappedSpec.GetWidth();

        public string PresentationHint => WrappedSpec.GetPresentationHint();

        public int NumberOfLines => WrappedSpec.GetNumberOfLinesWithDefault();

        #endregion

        private INakedObjectAdapter SafeGetValue(IActionParameterFacade parm, object rawValue) {
            try {
                return GetValue(parm, rawValue);
            }
            catch (Exception) {
                return null;
            }
        }

        private INakedObjectAdapter GetValue(IActionParameterFacade parm, object rawValue) {
            if (rawValue == null || rawValue is string s && string.IsNullOrEmpty(s)) {
                return null;
            }

            if (parm.Specification.IsParseable) {
                return parm.WrappedSpec().Spec.GetFacet<IParseableFacet>().ParseTextEntry((string) rawValue, framework.NakedObjectManager);
            }

            var collectionParm = parm.WrappedSpec() as IOneToManyActionParameterSpec;

            if (collectionParm != null && collectionParm.ElementSpec.IsParseable) {
                var stringArray = rawValue as string[];
                if (stringArray == null || !stringArray.Any()) {
                    return null;
                }

                var eSpec = collectionParm.ElementSpec;

                var objectArray = stringArray.Select(i => i == null ? null : eSpec.GetFacet<IParseableFacet>().ParseTextEntry(i, framework.NakedObjectManager).Object).Where(o => o != null).ToArray();

                if (!objectArray.Any()) {
                    return null;
                }

                var typedArray = Array.CreateInstance(objectArray.First().GetType(), objectArray.Length);

                Array.Copy(objectArray, typedArray, typedArray.Length);

                return framework.GetNakedObject(typedArray);
            }

            return framework.GetNakedObject(rawValue);
        }

        private Tuple<string, ITypeFacade> WrapChoiceParm(Tuple<string, IObjectSpec> parm) => new Tuple<string, ITypeFacade>(parm.Item1, new TypeFacade(parm.Item2, FrameworkFacade, framework));

        public override bool Equals(object obj) => obj is ActionParameterFacade apf && Equals(apf);

        public bool Equals(ActionParameterFacade other) {
            if (ReferenceEquals(null, other)) { return false; }

            return ReferenceEquals(this, other) || Equals(other.WrappedSpec, WrappedSpec);
        }

        public override int GetHashCode() => WrappedSpec != null ? WrappedSpec.GetHashCode() : 0;
    }
}