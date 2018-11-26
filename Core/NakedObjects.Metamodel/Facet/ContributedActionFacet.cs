// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class ContributedActionFacet : FacetAbstract, IContributedActionFacet {
        private readonly List<Tuple<IObjectSpecImmutable, string, string>> collectionContributees = new List<Tuple<IObjectSpecImmutable, string, string>>();
        private readonly List<Tuple<IObjectSpecImmutable, string, string>> objectContributees = new List<Tuple<IObjectSpecImmutable, string, string>>();

        public ContributedActionFacet(ISpecification holder)
            : base(typeof (IContributedActionFacet), holder) {}

        #region IContributedActionFacet Members

        public bool IsContributedTo(IObjectSpecImmutable spec) {
            //return objectContributees.Select(t => t.Item1).Any(spec.IsOfType);

            foreach (var t in objectContributees) {
                var s = t.Item1;
                if (spec.IsOfType(s)) {
                    return true;
                }
            }

            return false;
        }

        public bool IsContributedToCollectionOf(IObjectSpecImmutable spec) {
            //return collectionContributees.Select(t => t.Item1).Any(spec.IsOfType);

            foreach (var t in collectionContributees) {
                var s = t.Item1;
                if (spec.IsOfType(s)) {
                    return true;
                }
            }

            return false;
        }

        public string SubMenuWhenContributedTo(IObjectSpecImmutable spec) {
            return FindContributee(spec).Item2;
        }

        public string IdWhenContributedTo(IObjectSpecImmutable spec) {
            return FindContributee(spec).Item3;
        }

        #endregion

        public void AddObjectContributee(IObjectSpecImmutable type, string subMenu, string id) {
            objectContributees.Add(new Tuple<IObjectSpecImmutable, string, string>(type, subMenu, id));
        }

        //Here the type is the ElementType of the collection, not the type of collection.
        public void AddCollectionContributee(IObjectSpecImmutable type, string subMenu, string id) {
            collectionContributees.Add(new Tuple<IObjectSpecImmutable, string, string>(type, subMenu, id));
        }

        private Tuple<IObjectSpecImmutable, string, string> FindContributee(IObjectSpecImmutable spec) {
            if (!IsContributedTo(spec)) {
                throw new Exception("Action is not contributed to " + spec.Type);
            }
            Tuple<IObjectSpecImmutable, string, string> tuple = objectContributees.First(t => spec.IsOfType(t.Item1));
            return tuple;
        }
    }
}