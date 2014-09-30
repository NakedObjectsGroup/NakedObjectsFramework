// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Collections.Modify;
using NakedObjects.Architecture.Persist;
using NakedObjects.Core.Context;
using NakedObjects.Core.Persist;

namespace NakedObjects.Reflector.DotNet.Facets.Collections {
    public class DotNetGenericCollectionFacet<T> : CollectionFacetAbstract {
        public DotNetGenericCollectionFacet(IFacetHolder holder, Type elementClass, bool isASet)
            : base(holder, elementClass, isASet) {}

        protected static ICollection<T> AsGenericCollection(INakedObject collection) {
            return (ICollection<T>) collection.Object;
        }

        public override IEnumerable<INakedObject> AsEnumerable(INakedObject collection, INakedObjectManager manager) {
            return AsGenericCollection(collection).Select(arg => manager.CreateAdapter(arg, null, null));
        }

        public override IQueryable AsQueryable(INakedObject collection) {
            return AsGenericCollection(collection).AsQueryable();
        }

        public override bool Contains(INakedObject collection, INakedObject element) {
            return AsGenericCollection(collection).Contains((T) element.Object);
        }

        public override INakedObject Page(int page, int size, INakedObject collection, ILifecycleManager persistor, bool forceEnumerable) {
            return persistor.CreateAdapter(AsGenericCollection(collection).Skip((page - 1) * size).Take(size).ToList(), null, null);
        }

        public override void Init(INakedObject collection, INakedObject[] initData) {
            ICollection<T> wrappedCollection = AsGenericCollection(collection);
            IList<T> newData = initData.Select(x => x.GetDomainObject<T>()).ToList();

            List<T> toAdd = newData.Where(obj => !wrappedCollection.Contains(obj)).ToList();
            toAdd.ForEach(wrappedCollection.Add);

            List<T> toRemove = wrappedCollection.Where(obj => !newData.Contains(obj)).ToList();
            toRemove.ForEach(obj => wrappedCollection.Remove(obj));
        }
    }
}