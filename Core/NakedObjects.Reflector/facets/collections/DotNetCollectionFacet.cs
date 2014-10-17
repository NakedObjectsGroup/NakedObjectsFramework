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
using NakedObjects.Architecture.Facets.Collections.Modify;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Reflector.DotNet.Facets.Collections {
    public class DotNetCollectionFacet : CollectionFacetAbstract {
        public DotNetCollectionFacet(ISpecification holder)
            : base(holder) {}

        public DotNetCollectionFacet(ISpecification holder, Type elementType)
            : base(holder, elementType, false) {}

        public override bool IsQueryable {
            get { return false; }
        }

        protected static IList AsCollection(INakedObject collection) {
            return (IList) collection.Object;
        }

        public override IEnumerable<INakedObject> AsEnumerable(INakedObject collection, INakedObjectManager manager) {
            return AsCollection(collection).Cast<object>().Select(domainObject => manager.CreateAdapter(domainObject, null, null));
        }

        public override IQueryable AsQueryable(INakedObject collection) {
            return AsCollection(collection).AsQueryable();
        }

        public override bool Contains(INakedObject collection, INakedObject element) {
            return AsCollection(collection).Contains(element.Object);
        }

        private IEnumerable PageInternal(int page, int size, INakedObject collection, INakedObjectManager manager) {
            int firstIndex = (page - 1)*size;
            for (int index = firstIndex; index < firstIndex + size; index++) {
                if (index >= AsEnumerable(collection, manager).Count()) {
                    yield break;
                }
                yield return AsCollection(collection)[index];
            }
        }

        public override INakedObject Page(int page, int size, INakedObject collection, INakedObjectManager manager, bool forceEnumerable) {
            return manager.CreateAdapter(PageInternal(page, size, collection, manager), null, null);
        }

        public override void Init(INakedObject collection, INakedObject[] initData) {
            IList wrappedCollection = AsCollection(collection);

            List<object> toAdd = initData.Select(no => no.Object).Where(obj => !wrappedCollection.Contains(obj)).ToList();
            toAdd.ForEach(obj => wrappedCollection.Add(obj));

            List<object> toRemove = wrappedCollection.Cast<object>().Where(o => !initData.Select(x => x.Object).Contains(o)).ToList();
            toRemove.ForEach(wrappedCollection.Remove);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}