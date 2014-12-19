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

        private readonly Dictionary<IObjectSpecImmutable, string> contributees = new Dictionary<IObjectSpecImmutable, string>();


        public ContributedActionFacet(ISpecification holder)
            : base(typeof (IContributedActionFacet), holder) { }


        public void AddContributee(IObjectSpecImmutable type, string subMenu) {
            contributees.Add(type, subMenu);
        }

        #region IContributedActionFacet Members

        public bool ContributedTo(IObjectSpecImmutable spec) {
            return contributees.Keys.Any(spec.IsOfType); // IsOfType should handle sub-types correctly
        }

        public string SubMenuWhenContributedTo(IObjectSpecImmutable spec) {
            if (!ContributedTo(spec)) {
                throw new Exception("Action is not contributed to " + spec.Type);
            }
            var contributeeSpec = contributees.Keys.First(spec.IsOfType);
            return contributees[contributeeSpec];
        }

        #endregion
    }
}