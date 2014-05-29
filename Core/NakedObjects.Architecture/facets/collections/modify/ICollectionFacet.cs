// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Facets.Collections.Modify {
    public interface ICollectionFacet : IFacet {
        bool IsASet { get; }
        bool Contains(INakedObject collection, INakedObject element);
        void Init(INakedObject collection, INakedObject[] initData);
        INakedObject Page(int page, int size, INakedObject collection, bool forceEnumerable);
        IEnumerable<INakedObject> AsEnumerable(INakedObject collection);
        IQueryable AsQueryable(INakedObject objectRepresentingCollection);
    }

    // Copyright (c) Naked Objects Group Ltd.
}