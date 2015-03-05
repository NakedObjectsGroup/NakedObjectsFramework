// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    internal class GenericIEnumerableFacet : CollectionFacetAbstract {
        internal class IteratorWrapper<T> : IEnumerable<T> {
            private readonly IEnumerable iterator;

            public IteratorWrapper(IEnumerable iterator) {
                this.iterator = iterator;
            }

            public IEnumerator<T> GetEnumerator() {
                // do not convert to Linq - possible issue with EF cast to object 
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var o in iterator) {
                    yield return (T) o;
                }
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }
        }

        public GenericIEnumerableFacet(ISpecification holder, bool isASet)
            : base(holder, isASet) {}

        public GenericIEnumerableFacet(ISpecification holder)
            : base(holder, false) {}

        public override bool IsQueryable {
            get { return false; }
        }

        protected static IEnumerable<T> AsGenericIEnumerable<T>(INakedObject collection) {
            var objectType = collection.Object.GetType();

            if (objectType.GenericTypeArguments.Count() == 1) {
                return (IEnumerable<T>) collection.Object;
            }

            return new IteratorWrapper<T>((IEnumerable) collection.Object);
        }

        public INakedObject PageInternal<T>(int page, int size, INakedObject collection, INakedObjectManager manager, bool forceEnumerable) {
            return manager.CreateAdapter(AsGenericIEnumerable<T>(collection).Skip((page - 1)*size).Take(size).ToList(), null, null);
        }

        public IEnumerable<INakedObject> AsEnumerableInternal<T>(INakedObject collection, INakedObjectManager manager) {
            return AsGenericIEnumerable<T>(collection).Select(arg => manager.CreateAdapter(arg, null, null));
        }

        public IQueryable AsQueryableInternal<T>(INakedObject collection) {
            return AsGenericIEnumerable<T>(collection).AsQueryable();
        }

        public bool ContainsInternal<T>(INakedObject collection, INakedObject element) {
            return AsGenericIEnumerable<T>(collection).Contains((T) element.Object);
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
            IList newCollection = CollectionUtils.CloneCollectionAndPopulate(collection.Object, initData.Select(no => no.Object));
            collection.ReplacePoco(newCollection);
        }
    }
}