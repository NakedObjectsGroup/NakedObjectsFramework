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
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Util;

namespace NakedFramework.Metamodel.Facet; 

[Serializable]
public sealed class GenericIEnumerableFacet : CollectionFacetAbstract {
    public GenericIEnumerableFacet(ISpecification holder, bool isASet)
        : base(holder, isASet) { }

    public GenericIEnumerableFacet(ISpecification holder)
        : base(holder, false) { }

    public override bool IsQueryable => false;

    private static IEnumerable<T> AsGenericIEnumerable<T>(INakedObjectAdapter collection) {
        var objectType = collection.Object.GetType();

        if (objectType.GenericTypeArguments.Length == 1) {
            return (IEnumerable<T>) collection.Object;
        }

        return new IteratorWrapper<T>((IEnumerable) collection.Object);
    }

    public static INakedObjectAdapter PageInternal<T>(int page, int size, INakedObjectAdapter collection, INakedObjectManager manager, bool forceEnumerable) => manager.CreateAdapter(AsGenericIEnumerable<T>(collection).Skip((page - 1) * size).Take(size).ToList(), null, null);

    public static IEnumerable<INakedObjectAdapter> AsEnumerableInternal<T>(INakedObjectAdapter collection, INakedObjectManager manager) => AsGenericIEnumerable<T>(collection).Select(arg => manager.CreateAdapter(arg, null, null));

    public static IQueryable AsQueryableInternal<T>(INakedObjectAdapter collection) => AsGenericIEnumerable<T>(collection).AsQueryable();

    public static bool ContainsInternal<T>(INakedObjectAdapter collection, INakedObjectAdapter element) => AsGenericIEnumerable<T>(collection).Contains((T) element.Object);

    public override IEnumerable<INakedObjectAdapter> AsEnumerable(INakedObjectAdapter collection, INakedObjectManager manager) => (IEnumerable<INakedObjectAdapter>) Call("AsEnumerableInternal", collection, collection, manager);

    public override IQueryable AsQueryable(INakedObjectAdapter collection) => (IQueryable) Call("AsQueryableInternal", collection, collection);

    public override bool Contains(INakedObjectAdapter collection, INakedObjectAdapter element) => (bool) Call("ContainsInternal", collection, collection, element);

    public override INakedObjectAdapter Page(int page, int size, INakedObjectAdapter collection, INakedObjectManager manager, bool forceEnumerable) => (INakedObjectAdapter) Call("PageInternal", collection, page, size, collection, manager, forceEnumerable);

    public override void Init(INakedObjectAdapter collection, INakedObjectAdapter[] initData) {
        var newCollection = CollectionUtils.CloneCollectionAndPopulate(collection.Object, initData.Select(no => no.Object));
        collection.ReplacePoco(newCollection);
    }

    #region Nested type: IteratorWrapper

    public sealed class IteratorWrapper<T> : IEnumerable<T> {
        private readonly IEnumerable iterator;

        public IteratorWrapper(IEnumerable iterator) => this.iterator = iterator;

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator() {
            // do not convert to Linq - possible issue with EF cast to object 
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var o in iterator) {
                yield return (T) o;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }

    #endregion
}