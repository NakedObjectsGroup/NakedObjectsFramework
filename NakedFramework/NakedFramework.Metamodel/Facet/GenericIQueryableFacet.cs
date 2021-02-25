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
using NakedFramework.Architecture.Spec;
using NakedObjects.Core.Util;
using NakedObjects.Core.Util.Query;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class GenericIQueryableFacet : CollectionFacetAbstract {
        public GenericIQueryableFacet(ISpecification holder)
            : this(holder, false) { }

        public GenericIQueryableFacet(ISpecification holder, bool isASet)
            : base(holder, isASet) { }

        public override bool IsQueryable => true;

        private static IQueryable<T> AsGenericIQueryable<T>(INakedObjectAdapter collection) {
            var queryable = (IQueryable<T>) collection.Object;
            return queryable.IsOrdered() ? queryable : queryable.OrderBy(x => "");
        }

        public INakedObjectAdapter PageInternal<T>(int page, int size, INakedObjectAdapter collection, INakedObjectManager manager, bool forceEnumerable) {
            // page = 0 causes empty collection to be returned
            IEnumerable<T> newCollection = page == 0 ? AsGenericIQueryable<T>(collection).Take(0) : AsGenericIQueryable<T>(collection).Skip((page - 1) * size).Take(size);
            if (forceEnumerable) {
                newCollection = newCollection.ToList();
            }

            return manager.CreateAdapter(newCollection, null, null);
        }

        public IEnumerable<INakedObjectAdapter> AsEnumerableInternal<T>(INakedObjectAdapter collection, INakedObjectManager manager) => AsGenericIQueryable<T>(collection).AsEnumerable().Select(arg => manager.CreateAdapter(arg, null, null));

        public IQueryable AsQueryableInternal<T>(INakedObjectAdapter collection) => AsGenericIQueryable<T>(collection);

        public bool ContainsInternal<T>(INakedObjectAdapter collection, INakedObjectAdapter element) => AsGenericIQueryable<T>(collection).Contains((T) element.Object);

        public override INakedObjectAdapter Page(int page, int size, INakedObjectAdapter collection, INakedObjectManager manager, bool forceEnumerable) => (INakedObjectAdapter) Call("PageInternal", collection, page, size, collection, manager, forceEnumerable);

        public override IEnumerable<INakedObjectAdapter> AsEnumerable(INakedObjectAdapter collection, INakedObjectManager manager) => (IEnumerable<INakedObjectAdapter>) Call("AsEnumerableInternal", collection, collection, manager);

        public override IQueryable AsQueryable(INakedObjectAdapter collection) => (IQueryable) Call("AsQueryableInternal", collection, collection);

        public override void Init(INakedObjectAdapter collection, INakedObjectAdapter[] initData) {
            var newCollection = CollectionUtils.CloneCollectionAndPopulate(collection.Object, initData.Select(no => no.Object));
            collection.ReplacePoco(newCollection.AsQueryable());
        }

        public override bool Contains(INakedObjectAdapter collection, INakedObjectAdapter element) => (bool) Call("ContainsInternal", collection, collection, element);
    }
}