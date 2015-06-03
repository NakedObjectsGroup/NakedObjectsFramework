// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core;
using NakedObjects.Core.Reflect;
using NakedObjects.Facade.Contexts;
using NakedObjects.Facade.Utility;
using NakedObjects.Facade.Impl.Utility;

namespace NakedObjects.Facade.Impl {
    public class ActionParameterFacade : IActionParameterFacade {
        private readonly INakedObjectsFramework framework;
        private readonly IActionParameterSpec nakedObjectActionParameter;
        private readonly string overloadedUniqueId;

        public ActionParameterFacade(IActionParameterSpec nakedObjectActionParameter, IFrameworkFacade surface, INakedObjectsFramework framework, string overloadedUniqueId) {
            SurfaceUtils.AssertNotNull(nakedObjectActionParameter, "Action Parameter is null");
            SurfaceUtils.AssertNotNull(framework, "framework is null");
            SurfaceUtils.AssertNotNull(overloadedUniqueId, "overloadedUniqueId is null");
            SurfaceUtils.AssertNotNull(surface, "surface is null");

            this.nakedObjectActionParameter = nakedObjectActionParameter;
            this.framework = framework;
            this.overloadedUniqueId = overloadedUniqueId;
            Surface = surface;
        }

        public IActionParameterSpec WrappedSpec {
            get { return nakedObjectActionParameter; }
        }

        #region IActionParameterFacade Members

        public IDictionary<string, object> ExtensionData {
            get {
                var extData = new Dictionary<string, object>();

                if (nakedObjectActionParameter.ContainsFacet<IPresentationHintFacet>()) {
                    extData[IdConstants.PresentationHint] = nakedObjectActionParameter.GetFacet<IPresentationHintFacet>().Value;
                }

                return extData.Any() ? extData : null;
            }
        }

        public string Name {
            get { return nakedObjectActionParameter.Name; }
        }

        public string Description {
            get { return nakedObjectActionParameter.Description; }
        }

        public bool IsMandatory {
            get { return nakedObjectActionParameter.IsMandatory; }
        }

        public int? MaxLength {
            get {
                var facet = nakedObjectActionParameter.GetFacet<IMaxLengthFacet>();
                return facet != null ? (int?) facet.Value : null;
            }
        }

        public string Pattern {
            get {
                var facet = nakedObjectActionParameter.GetFacet<IRegExFacet>();
                return facet != null ? facet.Pattern.ToString() : null;
            }
        }

        public string Mask {
            get {
                var facet = nakedObjectActionParameter.GetFacet<IMaskFacet>();
                return facet != null ? facet.Value : null;
            }
        }

        public int AutoCompleteMinLength {
            get {
                var facet = nakedObjectActionParameter.GetFacet<IAutoCompleteFacet>();
                return facet != null ? facet.MinLength : 0;
            }
        }

        public int Number {
            get { return nakedObjectActionParameter.Number; }
        }

        public ITypeFacade Specification {
            get { return new TypeFacade(nakedObjectActionParameter.Spec, Surface, framework); }
        }

        public ITypeFacade ElementType {
            get {
                var parm = nakedObjectActionParameter as IOneToManyActionParameterSpec;
                var elementSpec = parm == null ? null : parm.ElementSpec;
                return elementSpec == null ? null : new TypeFacade(elementSpec, Surface, framework);
            }
        }

        public IActionFacade Action {
            get { return new ActionFacade(nakedObjectActionParameter.Action, Surface, framework, overloadedUniqueId ?? ""); }
        }

        public string Id {
            get { return nakedObjectActionParameter.Id; }
        }

        public Choices IsChoicesEnabled {
            get {
                if (nakedObjectActionParameter.IsMultipleChoicesEnabled) {
                    return Choices.Multiple;
                }
                return nakedObjectActionParameter.IsChoicesEnabled ? Choices.Single : Choices.NotEnabled;
            }
        }

        public bool IsAutoCompleteEnabled {
            get { return nakedObjectActionParameter.IsAutoCompleteEnabled; }
        }

        public IObjectFacade[] GetChoices(IObjectFacade nakedObject, IDictionary<string, object> parameterNameValues) {
            var otherParms = parameterNameValues == null ? null : parameterNameValues.Select(kvp => new {kvp.Key, kvp.Value, parm = Action.Parameters.Single(p => p.Id == kvp.Key)});

            var pnv = otherParms == null ? null : otherParms.ToDictionary(a => a.Key, a => GetValue(a.parm, a.Value));

            return nakedObjectActionParameter.GetChoices(((ObjectFacade) nakedObject).WrappedNakedObject, pnv).Select(no => ObjectFacade.Wrap(no, Surface, framework)).Cast<IObjectFacade>().ToArray();
        }

        public Tuple<string, ITypeFacade>[] GetChoicesParameters() {
            return nakedObjectActionParameter.GetChoicesParameters().Select(WrapChoiceParm).ToArray();
        }

        public string GetMaskedValue(IObjectFacade valueNakedObject) {
            var mask = nakedObjectActionParameter.GetFacet<IMaskFacet>();

            if (valueNakedObject == null) {
                return null;
            }
            var no = ((ObjectFacade) valueNakedObject).WrappedNakedObject;
            return mask != null ? no.Spec.GetFacet<ITitleFacet>().GetTitleWithMask(mask.Value, no, framework.NakedObjectManager) : no.TitleString();
        }

        public IConsentFacade IsValid(IObjectFacade target, object value) {
            var t = ((ObjectFacade) target).WrappedNakedObject;

            IConsent consent;
            try {
                var v = GetValue(this, value);
                consent = nakedObjectActionParameter.IsValid(t, v);
            }
            catch (InvalidEntryException) {
                consent = new Veto("Invalid Entry"); // todo i18n
            }
            catch (Exception e) {
                consent = new Veto(e.Message); // todo i18n
            }

            return new ConsentFacade(consent);
        }

        public Tuple<IObjectFacade, string>[] GetChoicesAndTitles(IObjectFacade nakedObject, IDictionary<string, object> parameterNameValues) {
            var choices = GetChoices(nakedObject, parameterNameValues);
            return choices.Select(c => new Tuple<IObjectFacade, string>(c, c.TitleString)).ToArray();
        }

        public IObjectFacade[] GetCompletions(IObjectFacade nakedObject, string autoCompleteParm) {
            return nakedObjectActionParameter.GetCompletions(((ObjectFacade) nakedObject).WrappedNakedObject, autoCompleteParm).Select(no => ObjectFacade.Wrap(no, Surface, framework)).Cast<IObjectFacade>().ToArray();
        }

        public bool DefaultTypeIsExplicit(IObjectFacade nakedObject) {
            return nakedObjectActionParameter.GetDefaultType(((ObjectFacade) nakedObject).WrappedNakedObject) == TypeOfDefaultValue.Explicit;
        }

        public IObjectFacade GetDefault(IObjectFacade nakedObject) {
            return ObjectFacade.Wrap(nakedObjectActionParameter.GetDefault(((ObjectFacade) nakedObject).WrappedNakedObject), Surface, framework);
        }

        public IFrameworkFacade Surface { get; set; }

        public bool IsFindMenuEnabled {
            get { return (nakedObjectActionParameter is IOneToOneActionParameterSpec) && ((IOneToOneActionParameterSpec) nakedObjectActionParameter).IsFindMenuEnabled; }
        }

        public Tuple<Regex, string> RegEx {
            get {
                var regEx = nakedObjectActionParameter.GetFacet<IRegExFacet>();
                return regEx == null ? null : new Tuple<Regex, string>(regEx.Pattern, regEx.FailureMessage);
            }
        }

        public Tuple<IConvertible, IConvertible, bool> Range {
            get {
                var rangeFacet = nakedObjectActionParameter.GetFacet<IRangeFacet>();
                return rangeFacet == null ? null : new Tuple<IConvertible, IConvertible, bool>(rangeFacet.Min, rangeFacet.Max, rangeFacet.IsDateRange);
            }
        }

        public bool IsAjax {
            get { return !nakedObjectActionParameter.ContainsFacet<IAjaxFacet>(); }
        }

        public bool IsPassword {
            get { return nakedObjectActionParameter.ContainsFacet<IPasswordFacet>(); }
        }

        public int TypicalLength {
            get {
                var typicalLength = nakedObjectActionParameter.GetFacet<ITypicalLengthFacet>();
                return typicalLength == null ? 0 : typicalLength.Value;
            }
        }

        public bool IsNullable {
            get { return nakedObjectActionParameter.ContainsFacet<INullableFacet>(); }
        }

        public int Width {
            get {
                var multiline = nakedObjectActionParameter.GetFacet<IMultiLineFacet>();
                return multiline == null ? 0 : multiline.Width;
            }
        }

        public int NumberOfLines {
            get {
                var multiline = nakedObjectActionParameter.GetFacet<IMultiLineFacet>();
                return multiline == null ? 1 : multiline.NumberOfLines;
            }
        }

        public string PresentationHint {
            get {
                var hintFacet = nakedObjectActionParameter.GetFacet<IPresentationHintFacet>();
                return hintFacet == null ? null : hintFacet.Value;
            }
        }

        #endregion

        private INakedObjectAdapter GetValue(IActionParameterFacade parm, object rawValue) {
            if (rawValue == null || rawValue is string && string.IsNullOrEmpty(rawValue as string)) {
                return null;
            }

            if (parm.Specification.IsParseable) {
                return nakedObjectActionParameter.Spec.GetFacet<IParseableFacet>().ParseTextEntry((string) rawValue, framework.NakedObjectManager);
            }
            var collectionParm = nakedObjectActionParameter as IOneToManyActionParameterSpec;

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

        private Tuple<string, ITypeFacade> WrapChoiceParm(Tuple<string, IObjectSpec> parm) {
            return new Tuple<string, ITypeFacade>(parm.Item1, new TypeFacade(parm.Item2, Surface, framework));
        }

        public override bool Equals(object obj) {
            var nakedObjectActionParameterWrapper = obj as ActionParameterFacade;
            if (nakedObjectActionParameterWrapper != null) {
                return Equals(nakedObjectActionParameterWrapper);
            }
            return false;
        }

        public bool Equals(ActionParameterFacade other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return Equals(other.nakedObjectActionParameter, nakedObjectActionParameter);
        }

        public override int GetHashCode() {
            return (nakedObjectActionParameter != null ? nakedObjectActionParameter.GetHashCode() : 0);
        }
    }
}