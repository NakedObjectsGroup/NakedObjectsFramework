// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Security {
    public interface IAuthenticationManager {
        string[] LogonNames { get; }

        ISession Authenticate();

        void CloseSession(ISession session);

        void Init();
    }

    // Copyright (c) Naked Objects Group Ltd.
}