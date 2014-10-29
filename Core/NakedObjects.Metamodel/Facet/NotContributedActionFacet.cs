// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Metamodel.Facet {
    public class NotContributedActionFacet : FacetAbstract, INotContributedActionFacet {
        public NotContributedActionFacet(ISpecification holder, IObjectSpecImmutable[] notContributedToTypes) 
            : base(typeof (INotContributedActionFacet), holder) {
            this.notContributedToTypes.AddRange(notContributedToTypes);
        }

        #region INotContributedActionFacet Members

        private readonly List<IObjectSpecImmutable> notContributedToTypes = new List<IObjectSpecImmutable>();

        public bool NotContributedTo(IObjectSpecImmutable spec) {
            return NeverContributed() || notContributedToTypes.Any(spec.IsOfType);
        }

        public bool NeverContributed() {
            return !notContributedToTypes.Any();
        }

        #endregion
    }
}