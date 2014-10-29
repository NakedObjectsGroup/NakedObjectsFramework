// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Meta.Facet {
    public class GenericCollectionFacet : CollectionFacetAbstract {

        public GenericCollectionFacet(ISpecification holder)
            : base(holder, false) { }

        public GenericCollectionFacet(ISpecification holder, bool isASet)
            : base(holder, isASet) {}

        public override bool IsQueryable {
            get { return false; }
        }

        protected static ICollection<T> AsGenericCollection<T>(INakedObject collection) {
            return (ICollection<T>) collection.Object;
        }

        public IEnumerable<INakedObject> AsEnumerableInternal<T>(INakedObject collection, INakedObjectManager manager) {
            return AsGenericCollection<T>(collection).Select(arg => manager.CreateAdapter(arg, null, null));
        }

        public IQueryable AsQueryableInternal<T>(INakedObject collection) {
            return AsGenericCollection<T>(collection).AsQueryable();
        }

        public bool ContainsInternal<T>(INakedObject collection, INakedObject element) {
            return AsGenericCollection<T>(collection).Contains((T) element.Object);
        }

        public INakedObject PageInternal<T>(int page, int size, INakedObject collection, INakedObjectManager manager, bool forceEnumerable) {
            return manager.CreateAdapter(AsGenericCollection<T>(collection).Skip((page - 1)*size).Take(size).ToList(), null, null);
        }

        public void InitInternal<T>(INakedObject collection, INakedObject[] initData) {
            ICollection<T> wrappedCollection = AsGenericCollection<T>(collection);
            IList<T> newData = initData.Select(x => x.GetDomainObject<T>()).ToList();

            List<T> toAdd = newData.Where(obj => !wrappedCollection.Contains(obj)).ToList();
            toAdd.ForEach(wrappedCollection.Add);

            List<T> toRemove = wrappedCollection.Where(obj => !newData.Contains(obj)).ToList();
            toRemove.ForEach(obj => wrappedCollection.Remove(obj));
        }


        public override IEnumerable<INakedObject> AsEnumerable(INakedObject collection, INakedObjectManager manager) {
            return (IEnumerable<INakedObject>) Call("AsEnumerableInternal", collection, collection, manager);
        }

        public override IQueryable AsQueryable(INakedObject collection) {
            return (IQueryable) Call("AsQueryableInternal", collection, collection);
        }

        public override bool Contains(INakedObject collection, INakedObject element) {
            return (bool) Call("ContainsInternal", collection, collection, element);
        }

        public override INakedObject Page(int page, int size, INakedObject collection, INakedObjectManager manager, bool forceEnumerable) {
            return (INakedObject) Call("PageInternal", collection, page, size, collection, manager, forceEnumerable);
        }

        public override void Init(INakedObject collection, INakedObject[] initData) {
            Call("InitInternal", collection, collection, initData);
        }
    }
}