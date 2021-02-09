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
using NakedObjects.Meta.Facet;

namespace NakedFunctions.Meta.Facet {
    [Serializable]
    public sealed class ContributedFunctionFacet : FacetAbstract, IContributedFunctionFacet {
        private readonly List<ITypeSpecImmutable> objectContributees = new();
        private readonly List<IObjectSpecImmutable> collectionContributees = new();

        public ContributedFunctionFacet(ISpecification holder, bool isContributedToObject) : base(typeof(IContributedFunctionFacet), holder) =>
            IsContributedToObject = isContributedToObject;

        public void AddContributee(ITypeSpecImmutable type) => objectContributees.Add(type);

        #region IContributedFunctionFacet Members

        public bool IsContributedTo(ITypeSpecImmutable spec) => objectContributees.Any(spec.IsOfType);
        public bool IsContributedToObject { get; }
        public bool IsContributedToCollection => collectionContributees.Any();

        #endregion

        public void AddCollectionContributee(IObjectSpecBuilder type) {
            collectionContributees.Add(type);
        }

        public bool IsContributedToCollectionOf(IObjectSpecImmutable objectSpec) => collectionContributees.Any(objectSpec.IsOfType);

        public void AddLocalCollectionContributee(IObjectSpecBuilder type, string pName) {
            throw new NotImplementedException();
        }
    }
}