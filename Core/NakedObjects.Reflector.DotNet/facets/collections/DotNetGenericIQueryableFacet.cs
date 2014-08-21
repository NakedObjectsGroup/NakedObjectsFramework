// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Collections.Modify;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Context;
using NakedObjects.Core.Persist;

namespace NakedObjects.Reflector.DotNet.Facets.Collections {
    public class DotNetGenericIQueryableFacet<T> : CollectionFacetAbstract {
        public DotNetGenericIQueryableFacet(IFacetHolder holder, Type elementClass, bool isASet)
            : base(holder, elementClass, isASet) {}


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

        public override INakedObject Page(int page, int size, INakedObject collection, INakedObjectPersistor persistor, bool forceEnumerable) {
            // page = 0 causes empty collection to be returned
            IEnumerable<T> newCollection = page == 0 ? AsGenericIQueryable(collection).Take(0) : AsGenericIQueryable(collection).Skip((page - 1)*size).Take(size);
            if (forceEnumerable) {
                newCollection = newCollection.ToList();
            }

            return NakedObjectsContext.ObjectPersistor.CreateAdapter(newCollection, null, null);
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