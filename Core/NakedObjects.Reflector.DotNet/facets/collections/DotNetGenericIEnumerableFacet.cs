// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Collections.Modify;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Context;
using NakedObjects.Core.Persist;

namespace NakedObjects.Reflector.DotNet.Facets.Collections {
    public class DotNetGenericIEnumerableFacet<T> : CollectionFacetAbstract {
        public DotNetGenericIEnumerableFacet(IFacetHolder holder, Type elementClass, bool isASet)
            : base(holder, elementClass, isASet) {}

        protected static IEnumerable<T> AsGenericIEnumerable(INakedObject collection) {
            return (IEnumerable<T>) collection.Object;
        }

        public override INakedObject Page(int page, int size, INakedObject collection, INakedObjectPersistor persistor, bool forceEnumerable) {
            return persistor.CreateAdapter(AsGenericIEnumerable(collection).Skip((page - 1) * size).Take(size).ToList(), null, null);
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