// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public class ContributedActionFacet : FacetAbstract, IContributedActionFacet {

        private readonly List<Tuple<IObjectSpecImmutable, string, string>> contributees = new List<Tuple<IObjectSpecImmutable, string, string>>();

        public ContributedActionFacet(ISpecification holder)
            : base(typeof (IContributedActionFacet), holder) { }

        public void AddContributee(IObjectSpecImmutable type, string subMenu, string id) {
            contributees.Add(new Tuple<IObjectSpecImmutable, string, string>(type, subMenu, id));
        }

        #region IContributedActionFacet Members

        public bool IsContributedTo(IObjectSpecImmutable spec) {
            return contributees.Select(t => t.Item1).Cast<IObjectSpecImmutable>().Any(spec.IsOfType); // IsOfType should handle sub-types correctly
        }

        public string SubMenuWhenContributedTo(IObjectSpecImmutable spec) {
            return FindContributee(spec).Item2;
        }

        public string IdWhenContributedTo(IObjectSpecImmutable spec) {
            return FindContributee(spec).Item3;
        }

        private Tuple<IObjectSpecImmutable, string, string> FindContributee(IObjectSpecImmutable spec) {
            if (!IsContributedTo(spec)) {
                throw new Exception("Action is not contributed to " + spec.Type);
            }
            var tuple = contributees.First(t => spec.IsOfType(t.Item1));
            return tuple;
        }
        #endregion

    }
}