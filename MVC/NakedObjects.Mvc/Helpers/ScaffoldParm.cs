// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Spec;
using NakedObjects.Surface;
using NakedObjects.Surface.Context;

namespace NakedObjects.Web.Mvc.Helpers {
    public class ScaffoldParm : INakedObjectActionParameterSurface {
        public ScaffoldParm(IActionParameterSpec spec) {
            WrappedSpec = spec;
        }

        public IActionParameterSpec WrappedSpec { get; set; }

        public static INakedObjectActionParameterSurface Wrap(IActionParameterSpec spec) {
            return new ScaffoldParm(spec);
        }

        public T GetScalarProperty<T>(ScalarProperty name) {
            throw new NotImplementedException();
        }

        public bool SupportsProperty(ScalarProperty name) {
            throw new NotImplementedException();
        }

        public IEnumerable<ScalarProperty> SupportedProperties() {
            throw new NotImplementedException();
        }

        public INakedObjectsSurface Surface {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }


        public INakedObjectSurface[] GetChoices(INakedObjectSurface nakedObject, IDictionary<string, object> parameterNameValues) {
            throw new NotImplementedException();
        }

        public Tuple<INakedObjectSurface, string>[] GetChoicesAndTitles(INakedObjectSurface nakedObject, IDictionary<string, object> parameterNameValues) {
            throw new NotImplementedException();
        }

       

        public INakedObjectSurface[] GetCompletions(INakedObjectSurface nakedObject, string autoCompleteParm) {
            throw new NotImplementedException();
        }

        public bool DefaultTypeIsExplicit(INakedObjectSurface nakedObject) {
            throw new NotImplementedException();
        }

        public INakedObjectSurface GetDefault(INakedObjectSurface nakedObject) {
            throw new NotImplementedException();
        }

        public Tuple<string, INakedObjectSpecificationSurface>[] GetChoicesParameters() {
            throw new NotImplementedException();
        }

        public string GetMaskedValue(INakedObjectSurface valueNakedObject) {
            throw new NotImplementedException();
        }

        public IConsentSurface IsValid(INakedObjectSurface target, object value) {
            throw new NotImplementedException();
        }

        

        public INakedObjectSpecificationSurface Specification {
            get { throw new NotImplementedException(); }
        }

        public INakedObjectSpecificationSurface ElementType {
            get { throw new NotImplementedException(); }
        }

        public INakedObjectActionSurface Action {
            get { throw new NotImplementedException(); }
        }

        public string Id {
            get { throw new NotImplementedException(); }
        }

        public Choices IsChoicesEnabled {
            get { throw new NotImplementedException(); }
        }

        public bool IsAutoCompleteEnabled {
            get { throw new NotImplementedException(); }
        }
    }
}