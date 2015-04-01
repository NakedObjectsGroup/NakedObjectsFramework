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

namespace NakedObjects.Web.Mvc.Helpers {
    public class ScaffoldAssoc : INakedObjectAssociationSurface {
        public ScaffoldAssoc(IAssociationSpec spec) {
            WrappedSpec = spec;
        }

        public IAssociationSpec WrappedSpec { get; set; }

        public static INakedObjectAssociationSurface Wrap(IAssociationSpec spec) {
            return new ScaffoldAssoc(spec);
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

        public string Id {
            get { throw new NotImplementedException(); }
        }

        public IConsentSurface IsUsable(INakedObjectSurface target) {
            throw new NotImplementedException();
        }

        public INakedObjectSurface GetNakedObject(INakedObjectSurface target) {
            throw new NotImplementedException();
        }

        public bool IsVisible(INakedObjectSurface nakedObject) {
            throw new NotImplementedException();
        }

        public bool IsEager(INakedObjectSurface nakedObject) {
            throw new NotImplementedException();
        }

        public INakedObjectSurface[] GetChoices(INakedObjectSurface target, IDictionary<string, INakedObjectSurface> parameterNameValues) {
            throw new NotImplementedException();
        }

        public Tuple<string, INakedObjectSpecificationSurface>[] GetChoicesParameters() {
            throw new NotImplementedException();
        }

        public Tuple<INakedObjectSurface, string>[] GetChoicesAndTitles(INakedObjectSurface target, IDictionary<string, INakedObjectSurface> parameterNameValues) {
            throw new NotImplementedException();
        }

        public INakedObjectSurface[] GetCompletions(INakedObjectSurface target, string autoCompleteParm) {
            throw new NotImplementedException();
        }

        public string GetTitle(INakedObjectSurface nakedObject) {
            throw new NotImplementedException();
        }

        public int Count(INakedObjectSurface target) {
            throw new NotImplementedException();
        }

        public INakedObjectSpecificationSurface Specification {
            get { throw new NotImplementedException(); }
        }

        public INakedObjectSpecificationSurface ElementSpecification {
            get { throw new NotImplementedException(); }
        }

        public bool IsChoicesEnabled {
            get { throw new NotImplementedException(); }
        }

        public bool IsAutoCompleteEnabled {
            get { throw new NotImplementedException(); }
        }
    }
}