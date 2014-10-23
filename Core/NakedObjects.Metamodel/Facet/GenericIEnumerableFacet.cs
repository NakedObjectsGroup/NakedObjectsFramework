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
using NakedObjects.Core.Util.Reflection;

namespace NakedObjects.Metamodel.Facet {
    public class GenericIEnumerableFacet<T> : CollectionFacetAbstract {

        public GenericIEnumerableFacet(ISpecification holder)
            : this(holder, false) { }

        public GenericIEnumerableFacet(ISpecification holder, bool isASet)
            : base(holder,  isASet) {}

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
    }
}