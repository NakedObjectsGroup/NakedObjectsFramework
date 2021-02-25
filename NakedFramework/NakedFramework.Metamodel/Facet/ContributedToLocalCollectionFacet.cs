// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;

namespace NakedFramework.Metamodel.Facet {
    [Serializable]
    public sealed class ContributedToLocalCollectionFacet : FacetAbstract, IContributedToLocalCollectionFacet {
        private readonly List<(IObjectSpecImmutable spec, string id)> localCollectionContributees = new();

        public ContributedToLocalCollectionFacet(ISpecification holder)
            : base(typeof(IContributedToLocalCollectionFacet), holder) { }

        #region IContributedActionFacet Members

        public bool IsContributedToLocalCollectionOf(IObjectSpecImmutable objectSpec, string id) => localCollectionContributees.Where(t => t.id == id.ToLower()).Select(t => t.spec).Any(objectSpec.IsOfType);

        #endregion

        public void AddLocalCollectionContributee(IObjectSpecImmutable objectSpec, string id) => localCollectionContributees.Add((objectSpec, id.ToLower()));
    }
}