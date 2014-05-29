// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Facets.Collections.Modify {
    public abstract class CollectionFacetAbstract : FacetAbstract, ICollectionFacet {
        protected CollectionFacetAbstract(IFacetHolder holder)
            : base(typeof (ICollectionFacet), holder) {
            IsASet = false;
        }

        protected CollectionFacetAbstract(IFacetHolder holder, Type elementClass, bool isASet)
            : this(holder) {
            IsASet = isASet;
        }

        #region ICollectionFacet Members

        public abstract bool Contains(INakedObject collection, INakedObject element);
        public abstract void Init(INakedObject nakedObject, INakedObject[] initData);
        public abstract INakedObject Page(int page, int size, INakedObject collection, bool forceEnumerable);

        public bool IsASet { get; private set; }
        public abstract IEnumerable<INakedObject> AsEnumerable(INakedObject collection);
        public abstract IQueryable AsQueryable(INakedObject collection);

        #endregion
    }


    // Copyright (c) Naked Objects Group Ltd.
}