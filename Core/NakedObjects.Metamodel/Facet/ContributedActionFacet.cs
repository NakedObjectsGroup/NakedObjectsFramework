// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class ContributedActionFacet : FacetAbstract, IContributedActionFacet {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ContributedActionFacet));
        private readonly List<(IObjectSpecImmutable spec, string subMenu, string id)> collectionContributees = new List<(IObjectSpecImmutable, string, string)>();
        private readonly List<(IObjectSpecImmutable spec, string id)> localCollectionContributees = new List<(IObjectSpecImmutable, string)>();
        private readonly List<(IObjectSpecImmutable spec, string subMenu , string id)> objectContributees = new List<(IObjectSpecImmutable, string, string)>();

        public ContributedActionFacet(ISpecification holder)
            : base(typeof(IContributedActionFacet), holder) { }

        #region IContributedActionFacet Members

        public bool IsContributedTo(IObjectSpecImmutable objectSpec) => objectContributees.Select(t => t.spec).Any(objectSpec.IsOfType);

        public bool IsContributedToCollectionOf(IObjectSpecImmutable objectSpec) => collectionContributees.Select(t => t.spec).Any(objectSpec.IsOfType);

        public bool IsContributedToLocalCollectionOf(IObjectSpecImmutable objectSpec, string id) => localCollectionContributees.Where(t => t.id == id.ToLower()).Select(t => t.spec).Any(objectSpec.IsOfType);

        public string SubMenuWhenContributedTo(IObjectSpecImmutable objectSpec) => FindContributee(objectSpec).subMenu;

        public string IdWhenContributedTo(IObjectSpecImmutable objectSpec) => FindContributee(objectSpec).id;

        #endregion

        public void AddObjectContributee(IObjectSpecImmutable objectSpec, string subMenu, string id) => objectContributees.Add((objectSpec, subMenu, id));

        //Here the type is the ElementType of the collection, not the type of collection.
        public void AddCollectionContributee(IObjectSpecImmutable objectSpec, string subMenu, string id) => collectionContributees.Add((objectSpec, subMenu, id));

        public void AddLocalCollectionContributee(IObjectSpecImmutable objectSpec, string id) => localCollectionContributees.Add((objectSpec, id.ToLower()));

        private (IObjectSpecImmutable spec, string subMenu, string id) FindContributee(IObjectSpecImmutable objectSpec) =>
            IsContributedTo(objectSpec)
                ? objectContributees.First(t => objectSpec.IsOfType(t.spec))
                : throw new Exception(Log.LogAndReturn($"Action is not contributed to {objectSpec.Type}"));
    }
}