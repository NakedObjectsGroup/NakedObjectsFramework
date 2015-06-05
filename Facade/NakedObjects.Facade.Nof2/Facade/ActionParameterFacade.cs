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
using NakedObjects.Facade.Contexts;
using NakedObjects.Facade.Nof2.Contexts;
using org.nakedobjects.@object;

namespace NakedObjects.Facade.Nof2 {
    public class ActionParameterFacade : IActionParameterFacade {
        private readonly NakedObjectActionParameter nakedObjectActionParameter;
        private readonly Naked target;

        public ActionParameterFacade(NakedObjectActionParameter nakedObjectActionParameter, Naked target, IFrameworkFacade frameworkFacade) {
            this.nakedObjectActionParameter = nakedObjectActionParameter;
            this.target = target;
            FrameworkFacade = frameworkFacade;
        }

        public bool IsChoicesEnabled {
            get { return nakedObjectActionParameter.isChoicesEnabled(); }
        }

        #region IActionParameterFacade Members

        public ITypeFacade Specification {
            get { return new TypeFacade(nakedObjectActionParameter.getSpecification(), target, FrameworkFacade); }
        }

        public ITypeFacade ElementType { get; private set; }

        public IActionFacade Action {
            get { return new ActionFacade(nakedObjectActionParameter.getAction(), target, FrameworkFacade); }
        }

        public string Id {
            get { return nakedObjectActionParameter.getId(); }
        }

        Choices IActionParameterFacade.IsChoicesEnabled {
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

        public IObjectFacade[] GetChoices(IObjectFacade objectFacade, IDictionary<string, object> parameterNameValues) {
            //return GetChoices(objectFacade,  )
            return new IObjectFacade[] {};
        }

        public Tuple<IObjectFacade, string>[] GetChoicesAndTitles(IObjectFacade objectFacade, IDictionary<string, object> parameterNameValues) {
            throw new NotImplementedException();
        }

        public IObjectFacade[] GetCompletions(IObjectFacade objectFacade, string autoCompleteParm) {
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

        public bool DefaultTypeIsExplicit(IObjectFacade objectFacade) {
            return nakedObjectActionParameter.getDefault(((ObjectFacade) objectFacade).NakedObject) != null;
        }

        public IObjectFacade GetDefault(IObjectFacade objectFacade) {
            return new ObjectFacade(nakedObjectActionParameter.getDefault(((ObjectFacade) objectFacade).NakedObject), FrameworkFacade);
        }

        public Tuple<string, ITypeFacade>[] GetChoicesParameters() {
            return new Tuple<string, ITypeFacade>[] {};
        }

        public string GetMaskedValue(IObjectFacade objectFacade) {
            return objectFacade.TitleString;
        }

        public IConsentFacade IsValid(IObjectFacade target, object value) {
            throw new NotImplementedException();
        }

        public IFrameworkFacade FrameworkFacade { get; set; }

        #endregion

        public IObjectFacade[] GetChoices(IObjectFacade objectFacade, IDictionary<string, IObjectFacade> parameterNameValues) {
            return nakedObjectActionParameter.getChoices(((ObjectFacade) objectFacade).NakedObject).Select(no => new ObjectFacade(no, FrameworkFacade)).Cast<IObjectFacade>().ToArray();
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