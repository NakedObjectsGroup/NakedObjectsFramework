// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Persist {
    public interface INakedObjectTransactionManager : IRequiresSetup {
        void StartTransaction();

        bool FlushTransaction();

        void AbortTransaction();

        void UserAbortTransaction();

        void EndTransaction();

        void AddCommand(IPersistenceCommand command);
        
    }


    // Copyright (c) Naked Objects Group Ltd.
}