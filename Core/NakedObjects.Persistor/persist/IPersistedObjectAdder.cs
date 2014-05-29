// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Persistor {
    public interface IPersistedObjectAdder {
        void AddPersistedObject(INakedObject nakedObject);

        void MadePersistent(INakedObject nakedObject);
    }

    // Copyright (c) Naked Objects Group Ltd.
}