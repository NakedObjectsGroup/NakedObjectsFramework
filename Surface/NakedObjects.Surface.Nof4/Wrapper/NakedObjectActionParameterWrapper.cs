// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Facets.AutoComplete;
using NakedObjects.Architecture.Facets.Presentation;
using NakedObjects.Architecture.Facets.Propparam.Validate.Mask;
using NakedObjects.Architecture.Facets.Propparam.Validate.MaxLength;
using NakedObjects.Architecture.Facets.Propparam.Validate.RegEx;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Surface.Utility;

namespace NakedObjects.Surface.Nof4.Wrapper {
    public class NakedObjectActionParameterWrapper : ScalarPropertyHolder, INakedObjectActionParameterSurface {
        private readonly INakedObjectsFramework framework;
        private readonly IActionParameterSpec nakedObjectActionParameter;
        private readonly string overloadedUniqueId;

        public NakedObjectActionParameterWrapper(IActionParameterSpec nakedObjectActionParameter, INakedObjectsSurface surface, INakedObjectsFramework framework, string overloadedUniqueId) {
            this.nakedObjectActionParameter = nakedObjectActionParameter;
            this.framework = framework;
            this.overloadedUniqueId = overloadedUniqueId;
            Surface = surface;
        }

        protected IDictionary<string, object> ExtensionData {
            get {
                var extData = new Dictionary<string, object>();

                if (nakedObjectActionParameter.ContainsFacet<IPresentationHintFacet>()) {
                    extData[PresentationHint] = nakedObjectActionParameter.GetFacet<IPresentationHintFacet>().Value;
                }

                return extData.Any() ? extData : null;
            }
        }

        public string Name {
            get { return nakedObjectActionParameter.GetName(); }
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

        #region INakedObjectActionParameterSurface Members

        public INakedObjectSpecificationSurface Specification {
            get { return new NakedObjectSpecificationWrapper(nakedObjectActionParameter.Spec, Surface, framework); }
        }

        public INakedObjectActionSurface Action {
            get { return new NakedObjectActionWrapper(nakedObjectActionParameter.Action, Surface, framework, overloadedUniqueId ?? ""); }
        }

        public string Id {
            get { return nakedObjectActionParameter.Id; }
        }

        public bool IsChoicesEnabled {
            get { return nakedObjectActionParameter.IsChoicesEnabled || nakedObjectActionParameter.IsMultipleChoicesEnabled; }
        }

        public bool IsAutoCompleteEnabled {
            get { return nakedObjectActionParameter.IsAutoCompleteEnabled; }
        }

        public INakedObjectSurface[] GetChoices(INakedObjectSurface nakedObject, IDictionary<string, INakedObjectSurface> parameterNameValues) {
            return nakedObjectActionParameter.GetChoices(((NakedObjectWrapper) nakedObject).WrappedNakedObject, null).Select(no => NakedObjectWrapper.Wrap(no, Surface, framework)).Cast<INakedObjectSurface>().ToArray();
        }

        public Tuple<string, INakedObjectSpecificationSurface>[] GetChoicesParameters() {
            return nakedObjectActionParameter.GetChoicesParameters().Select(WrapChoiceParm).ToArray();
        }

        public Tuple<INakedObjectSurface, string>[] GetChoicesAndTitles(INakedObjectSurface nakedObject, IDictionary<string, INakedObjectSurface> parameterNameValues) {
            var choices = GetChoices(nakedObject, parameterNameValues);
            return choices.Select(c => new Tuple<INakedObjectSurface, string>(c, c.TitleString())).ToArray();
        }

        public INakedObjectSurface[] GetCompletions(INakedObjectSurface nakedObject, string autoCompleteParm) {
            return nakedObjectActionParameter.GetCompletions(((NakedObjectWrapper) nakedObject).WrappedNakedObject, autoCompleteParm).Select(no => NakedObjectWrapper.Wrap(no, Surface, framework)).Cast<INakedObjectSurface>().ToArray();
        }

        public bool DefaultTypeIsExplicit(INakedObjectSurface nakedObject) {
            return nakedObjectActionParameter.GetDefaultType(((NakedObjectWrapper) nakedObject).WrappedNakedObject) == TypeOfDefaultValue.Explicit;
        }

        public INakedObjectSurface GetDefault(INakedObjectSurface nakedObject) {
            return NakedObjectWrapper.Wrap(nakedObjectActionParameter.GetDefault(((NakedObjectWrapper) nakedObject).WrappedNakedObject), Surface, framework);
        }

        public INakedObjectsSurface Surface { get; set; }

        #endregion

        private Tuple<string, INakedObjectSpecificationSurface> WrapChoiceParm(Tuple<string, IObjectSpec> parm) {
            return new Tuple<string, INakedObjectSpecificationSurface>(parm.Item1, new NakedObjectSpecificationWrapper(parm.Item2, Surface, framework));
        }

        public override bool Equals(object obj) {
            var nakedObjectActionParameterWrapper = obj as NakedObjectActionParameterWrapper;
            if (nakedObjectActionParameterWrapper != null) {
                return Equals(nakedObjectActionParameterWrapper);
            }
            return false;
        }

        public bool Equals(NakedObjectActionParameterWrapper other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.nakedObjectActionParameter, nakedObjectActionParameter);
        }

        public override int GetHashCode() {
            return (nakedObjectActionParameter != null ? nakedObjectActionParameter.GetHashCode() : 0);
        }

        public override object GetScalarProperty(ScalarProperty name) {
            switch (name) {
                case (ScalarProperty.Name):
                    return Name;
                case (ScalarProperty.Description):
                    return Description;
                case (ScalarProperty.IsMandatory):
                    return IsMandatory;
                case (ScalarProperty.MaxLength):
                    return MaxLength;
                case (ScalarProperty.Pattern):
                    return Pattern;
                case (ScalarProperty.Number):
                    return Number;
                case (ScalarProperty.Mask):
                    return Mask;
                case (ScalarProperty.AutoCompleteMinLength):
                    return AutoCompleteMinLength;
                case (ScalarProperty.ExtensionData):
                    return ExtensionData;
                default:
                    throw new NotImplementedException(string.Format("{0} doesn't support {1}", GetType(), name));
            }
        }
    }
}