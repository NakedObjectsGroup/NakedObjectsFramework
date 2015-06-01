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
using NakedObjects.Surface.Context;
using NakedObjects.Surface.Nof2.Context;
using org.nakedobjects.@object;

namespace NakedObjects.Surface.Nof2.Wrapper {
    public class NakedObjectActionParameterWrapper : INakedObjectActionParameterSurface {
        private readonly NakedObjectActionParameter nakedObjectActionParameter;
        private readonly Naked target;

        public NakedObjectActionParameterWrapper(NakedObjectActionParameter nakedObjectActionParameter, Naked target, IFrameworkFacade surface) {
            this.nakedObjectActionParameter = nakedObjectActionParameter;
            this.target = target;
            Surface = surface;
        }

        public bool IsChoicesEnabled {
            get { return nakedObjectActionParameter.isChoicesEnabled(); }
        }

        #region INakedObjectActionParameterSurface Members

        public INakedObjectSpecificationSurface Specification {
            get { return new NakedObjectSpecificationWrapper(nakedObjectActionParameter.getSpecification(), target, Surface); }
        }

        public INakedObjectSpecificationSurface ElementType { get; private set; }

        public INakedObjectActionSurface Action {
            get { return new NakedObjectActionWrapper(nakedObjectActionParameter.getAction(), target, Surface); }
        }

        public string Id {
            get { return nakedObjectActionParameter.getId(); }
        }

        Choices INakedObjectActionParameterSurface.IsChoicesEnabled {
            get { return IsChoicesEnabled ? Choices.Single : Choices.NotEnabled; }
        }

        public bool IsAutoCompleteEnabled {
            get { return false; }
        }

        public string PresentationHint { get; private set; }
        public Tuple<Regex, string> RegEx { get; private set; }
        public Tuple<IConvertible, IConvertible, bool> Range { get; private set; }
        public int NumberOfLines { get; private set; }
        public int Width { get; private set; }
        public int TypicalLength { get; private set; }
        public bool IsAjax { get; private set; }
        public bool IsNullable { get; private set; }
        public bool IsPassword { get; private set; }
        public bool IsFindMenuEnabled { get; private set; }

        public IObjectFacade[] GetChoices(IObjectFacade nakedObject, IDictionary<string, object> parameterNameValues) {
            //return GetChoices(nakedObject,  )
            return new IObjectFacade[] {};
        }

        public Tuple<IObjectFacade, string>[] GetChoicesAndTitles(IObjectFacade nakedObject, IDictionary<string, object> parameterNameValues) {
            throw new NotImplementedException();
        }

        public IObjectFacade[] GetCompletions(IObjectFacade nakedObject, string autoCompleteParm) {
            throw new NotImplementedException();
        }

        public string Name {
            get { return nakedObjectActionParameter.getName(); }
        }

        public string Description {
            get { return nakedObjectActionParameter.getDescription(); }
        }

        public bool IsMandatory {
            get { return nakedObjectActionParameter.isMandatory(); }
        }

        public int? MaxLength {
            get { return null; }
        }

        public string Pattern {
            get { return null; }
        }

        public int Number {
            get { return nakedObjectActionParameter.getNumber(); }
        }

        public string Mask { get; private set; }
        public int AutoCompleteMinLength { get; private set; }
        public IDictionary<string, object> ExtensionData { get; private set; }

        public bool DefaultTypeIsExplicit(IObjectFacade nakedObject) {
            return nakedObjectActionParameter.getDefault(((NakedObjectWrapper) nakedObject).NakedObject) != null;
        }

        public IObjectFacade GetDefault(IObjectFacade nakedObject) {
            return new NakedObjectWrapper(nakedObjectActionParameter.getDefault(((NakedObjectWrapper) nakedObject).NakedObject), Surface);
        }

        public Tuple<string, INakedObjectSpecificationSurface>[] GetChoicesParameters() {
            return new Tuple<string, INakedObjectSpecificationSurface>[] {};
        }

        public string GetMaskedValue(IObjectFacade valueNakedObject) {
            return valueNakedObject.TitleString;
        }

        public IConsentSurface IsValid(IObjectFacade target, object value) {
            throw new NotImplementedException();
        }

        public IFrameworkFacade Surface { get; set; }

        #endregion

        public IObjectFacade[] GetChoices(IObjectFacade nakedObject, IDictionary<string, IObjectFacade> parameterNameValues) {
            return nakedObjectActionParameter.getChoices(((NakedObjectWrapper) nakedObject).NakedObject).Select(no => new NakedObjectWrapper(no, Surface)).Cast<IObjectFacade>().ToArray();
        }

        public override bool Equals(object obj) {
            var nakedObjectActionParameterWrapper = obj as NakedObjectActionParameterWrapper;
            if (nakedObjectActionParameterWrapper != null) {
                return Equals(nakedObjectActionParameterWrapper);
            }
            return false;
        }

        public bool Equals(NakedObjectActionParameterWrapper other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return Equals(other.nakedObjectActionParameter, nakedObjectActionParameter);
        }

        public override int GetHashCode() {
            return (nakedObjectActionParameter != null ? nakedObjectActionParameter.GetHashCode() : 0);
        }

       
    }
}