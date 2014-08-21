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
using NakedObjects.Core.Context;

namespace NakedObjects.Reflector.DotNet.Facets.Collections {
    public class DotNetCollectionFacet : CollectionFacetAbstract {
        public DotNetCollectionFacet(IFacetHolder holder)
            : base(holder) {}

        public DotNetCollectionFacet(IFacetHolder holder, Type elementType)
            : base(holder, elementType, false) {}

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

        private IEnumerable PageInternal(int page, int size, INakedObject collection, INakedObjectPersistor persistor) {
            int firstIndex = (page - 1)*size;
            for (int index = firstIndex; index < firstIndex + size; index++) {
                if (index >= AsEnumerable(collection, persistor).Count()) {
                    yield break;
                }
                yield return AsCollection(collection)[index];
            }
        }

        public override INakedObject Page(int page, int size, INakedObject collection, INakedObjectPersistor persistor, bool forceEnumerable) {
            return persistor.CreateAdapter(PageInternal(page, size, collection, persistor), null, null);
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