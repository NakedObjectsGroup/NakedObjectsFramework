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

namespace NakedFramework.Metamodel.Facet;

[Serializable]
public class CollectionFacet : CollectionFacetAbstract {
    public CollectionFacet(ISpecification holder)
        : base(holder, false) { }

    public override bool IsQueryable => false;

    protected static IList AsCollection(INakedObjectAdapter collection) => (IList)collection.Object;

    public override IEnumerable<INakedObjectAdapter> AsEnumerable(INakedObjectAdapter collection, INakedObjectManager manager) => AsCollection(collection).Cast<object>().Select(domainObject => manager.CreateAdapter(domainObject, null, null));

    public override IQueryable AsQueryable(INakedObjectAdapter collection) => AsCollection(collection).AsQueryable();

    public override void Init(INakedObjectAdapter collection, INakedObjectAdapter[] initData) {
        var wrappedCollection = AsCollection(collection);

        var toAdd = initData.Select(no => no.Object).Where(obj => !wrappedCollection.Contains(obj)).ToList();
        toAdd.ForEach(obj => wrappedCollection.Add(obj));

        var toRemove = wrappedCollection.Cast<object>().Where(o => !initData.Select(x => x.Object).Contains(o)).ToList();
        toRemove.ForEach(wrappedCollection.Remove);
    }

    public override bool Contains(INakedObjectAdapter collection, INakedObjectAdapter element) => AsCollection(collection).Contains(element.Object);

    private IEnumerable PageInternal(int page, int size, INakedObjectAdapter collection, INakedObjectManager manager) {
        var firstIndex = (page - 1) * size;
        for (var index = firstIndex; index < firstIndex + size; index++) {
            if (index >= AsEnumerable(collection, manager).Count()) {
                yield break;
            }

            yield return AsCollection(collection)[index];
        }
    }

    public override INakedObjectAdapter Page(int page, int size, INakedObjectAdapter collection, INakedObjectManager manager, bool forceEnumerable) => manager.CreateAdapter(PageInternal(page, size, collection, manager), null, null);
}

// Copyright (c) Naked Objects Group Ltd.