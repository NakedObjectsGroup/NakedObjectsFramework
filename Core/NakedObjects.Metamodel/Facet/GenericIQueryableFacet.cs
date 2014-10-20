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
using System.Linq.Expressions;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;

namespace NakedObjects.Metamodel.Facet {
    public class GenericIQueryableFacet<T> : CollectionFacetAbstract {
        public GenericIQueryableFacet(ISpecification holder, Type elementClass, bool isASet)
            : base(holder, elementClass, isASet) {}

        public override bool IsQueryable {
            get { return true; }
        }


        private static bool IsOrdered(IQueryable queryable) {
            Expression expr = queryable.Expression;

            if (expr is MethodCallExpression) {
                MethodInfo method = (expr as MethodCallExpression).Method;
                return method.Name.StartsWith("OrderBy") || method.Name.StartsWith("ThenBy");
            }

            return false;
        }

        protected static IQueryable<T> AsGenericIQueryable(INakedObject collection) {
            var queryable = (IQueryable<T>) collection.Object;
            return IsOrdered(queryable) ? queryable : queryable.OrderBy(x => "");
        }

        public override INakedObject Page(int page, int size, INakedObject collection, INakedObjectManager manager, bool forceEnumerable) {
            // page = 0 causes empty collection to be returned
            IEnumerable<T> newCollection = page == 0 ? AsGenericIQueryable(collection).Take(0) : AsGenericIQueryable(collection).Skip((page - 1)*size).Take(size);
            if (forceEnumerable) {
                newCollection = newCollection.ToList();
            }

            return manager.CreateAdapter(newCollection, null, null);
        }

        public override IEnumerable<INakedObject> AsEnumerable(INakedObject collection, INakedObjectManager manager) {
            return AsGenericIQueryable(collection).AsEnumerable().Select(arg => manager.CreateAdapter(arg, null, null));
        }

        public override IQueryable AsQueryable(INakedObject collection) {
            return AsGenericIQueryable(collection);
        }

        public override bool Contains(INakedObject collection, INakedObject element) {
            return AsGenericIQueryable(collection).Contains((T) element.Object);
        }

        public override void Init(INakedObject collection, INakedObject[] initData) {
            IList newCollection = CollectionUtils.CloneCollectionAndPopulate(collection.Object, initData.Select(no => no.Object));
            collection.ReplacePoco(newCollection.AsQueryable());
        }
    }
}