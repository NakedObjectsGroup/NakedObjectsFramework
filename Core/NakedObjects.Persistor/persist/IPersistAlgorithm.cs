// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Security;

namespace NakedObjects.Persistor {
    public interface IPersistAlgorithm  {
        string Name { get; }
        void MakePersistent(INakedObject nakedObject, ILifecycleManager adders, ISession session);
    }

    // Copyright (c) Naked Objects Group Ltd.
}