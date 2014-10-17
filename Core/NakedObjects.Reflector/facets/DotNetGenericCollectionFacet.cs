// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Spec;
using NakedObjects.Metamodel.Facet;

namespace NakedObjects.Reflector.DotNet.Facets.Collections {
    public class DotNetGenericCollectionFacet<T> : CollectionFacetAbstract {
        public DotNetGenericCollectionFacet(ISpecification holder, Type elementClass, bool isASet)
            : base(holder, elementClass, isASet) {}

        public override bool IsQueryable {
            get { return false; }
        }

        protected static ICollection<T> AsGenericCollection(INakedObject collection) {
            return (ICollection<T>) collection.Object;
        }

        public override IEnumerable<INakedObject> AsEnumerable(INakedObject collection, INakedObjectManager manager) {
            return AsGenericCollection(collection).Select(arg => manager.CreateAdapter(arg, null, null));
        }

        public override IQueryable AsQueryable(INakedObject collection) {
            return AsGenericCollection(collection).AsQueryable();
        }

        public override bool Contains(INakedObject collection, INakedObject element) {
            return AsGenericCollection(collection).Contains((T) element.Object);
        }

        public override INakedObject Page(int page, int size, INakedObject collection, INakedObjectManager manager, bool forceEnumerable) {
            return manager.CreateAdapter(AsGenericCollection(collection).Skip((page - 1)*size).Take(size).ToList(), null, null);
        }

        public override void Init(INakedObject collection, INakedObject[] initData) {
            ICollection<T> wrappedCollection = AsGenericCollection(collection);
            IList<T> newData = initData.Select(x => x.GetDomainObject<T>()).ToList();

            List<T> toAdd = newData.Where(obj => !wrappedCollection.Contains(obj)).ToList();
            toAdd.ForEach(wrappedCollection.Add);

            List<T> toRemove = wrappedCollection.Where(obj => !newData.Contains(obj)).ToList();
            toRemove.ForEach(obj => wrappedCollection.Remove(obj));
        }
    }
}