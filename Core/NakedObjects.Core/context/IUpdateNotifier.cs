// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Core.Context {
    public interface IUpdateNotifier : IRequiresSetup {
        void AddChangedObject(INakedObject nakedObject);

        void AddDisposedObject(INakedObject nakedObject);

        IEnumerator<INakedObject> AllChangedObjects();

        IEnumerator<INakedObject> AllDisposedObjects();

        void EnsureEmpty();
    }


    // Copyright (c) Naked Objects Group Ltd.
}