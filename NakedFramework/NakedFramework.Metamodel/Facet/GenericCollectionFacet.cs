// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class GenericCollectionFacet : CollectionFacetAbstract {
        public GenericCollectionFacet(ISpecification holder)
            : base(holder, false) { }

        public GenericCollectionFacet(ISpecification holder, bool isASet)
            : base(holder, isASet) { }

        public override bool IsQueryable => false;

        private static ICollection<T> AsGenericCollection<T>(INakedObjectAdapter collection) => (ICollection<T>) collection.Object;

        public IEnumerable<INakedObjectAdapter> AsEnumerableInternal<T>(INakedObjectAdapter collection, INakedObjectManager manager) => AsGenericCollection<T>(collection).Select(arg => manager.CreateAdapter(arg, null, null));

        public IQueryable AsQueryableInternal<T>(INakedObjectAdapter collection) => AsGenericCollection<T>(collection).AsQueryable();

        public bool ContainsInternal<T>(INakedObjectAdapter collection, INakedObjectAdapter element) => AsGenericCollection<T>(collection).Contains((T) element.Object);

        public INakedObjectAdapter PageInternal<T>(int page, int size, INakedObjectAdapter collection, INakedObjectManager manager, bool forceEnumerable) => manager.CreateAdapter(AsGenericCollection<T>(collection).Skip((page - 1) * size).Take(size).ToList(), null, null);

        public void InitInternal<T>(INakedObjectAdapter collection, INakedObjectAdapter[] initData) {
            var wrappedCollection = AsGenericCollection<T>(collection);
            IList<T> newData = initData.Select(x => x.GetDomainObject<T>()).ToList();

            var toAdd = newData.Where(obj => !wrappedCollection.Contains(obj)).ToList();
            toAdd.ForEach(wrappedCollection.Add);

            var toRemove = wrappedCollection.Where(obj => !newData.Contains(obj)).ToList();
            toRemove.ForEach(obj => wrappedCollection.Remove(obj));
        }

        public override IEnumerable<INakedObjectAdapter> AsEnumerable(INakedObjectAdapter collection, INakedObjectManager manager) => (IEnumerable<INakedObjectAdapter>) Call("AsEnumerableInternal", collection, collection, manager);

        public override IQueryable AsQueryable(INakedObjectAdapter collection) => (IQueryable) Call("AsQueryableInternal", collection, collection);

        public override bool Contains(INakedObjectAdapter collection, INakedObjectAdapter element) => (bool) Call("ContainsInternal", collection, collection, element);

        public override INakedObjectAdapter Page(int page, int size, INakedObjectAdapter collection, INakedObjectManager manager, bool forceEnumerable) => (INakedObjectAdapter) Call("PageInternal", collection, page, size, collection, manager, forceEnumerable);

        public override void Init(INakedObjectAdapter collection, INakedObjectAdapter[] initData) => Call("InitInternal", collection, collection, initData);
    }
}