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
using NakedObjects.Architecture.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Collections {
    public class DotNetGenericIEnumerableFacet<T> : CollectionFacetAbstract {
        public DotNetGenericIEnumerableFacet(ISpecification holder, Type elementClass, bool isASet)
            : base(holder, elementClass, isASet) {}

        public override bool IsQueryable {
            get { return false; }
        }

        protected static IEnumerable<T> AsGenericIEnumerable(INakedObject collection) {
            return (IEnumerable<T>) collection.Object;
        }

        public override INakedObject Page(int page, int size, INakedObject collection, INakedObjectManager manager, bool forceEnumerable) {
            return manager.CreateAdapter(AsGenericIEnumerable(collection).Skip((page - 1)*size).Take(size).ToList(), null, null);
        }

        public override IEnumerable<INakedObject> AsEnumerable(INakedObject collection, INakedObjectManager manager) {
            return AsGenericIEnumerable(collection).Select(arg => manager.CreateAdapter(arg, null, null));
        }

        public override IQueryable AsQueryable(INakedObject collection) {
            return AsGenericIEnumerable(collection).AsQueryable();
        }

        public override bool Contains(INakedObject collection, INakedObject element) {
            return AsGenericIEnumerable(collection).Contains((T) element.Object);
        }

        public override void Init(INakedObject collection, INakedObject[] initData) {
            IList newCollection = CollectionUtils.CloneCollectionAndPopulate(collection.Object, initData.Select(no => no.Object));
            collection.ReplacePoco(newCollection);
        }
    }
}