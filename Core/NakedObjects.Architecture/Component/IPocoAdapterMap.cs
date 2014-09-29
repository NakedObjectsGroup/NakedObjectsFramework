// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Core.Adapter.Map {
    public interface IPocoAdapterMap : IEnumerable<INakedObject> {
        void Add(object obj, INakedObject adapter);

        bool ContainsObject(object obj);

        INakedObject GetObject(object obj);

        void Reset();

        void Shutdown();

        void Remove(INakedObject nakedObject);
    }

    // Copyright (c) Naked Objects Group Ltd.
}