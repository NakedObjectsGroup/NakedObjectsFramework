// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Security {
    public interface IConnectionManager {
        void Connect(ISession session);

        void Disconnect(ISession session);

        void Shutdown();
    }

    // Copyright (c) Naked Objects Group Ltd.
}