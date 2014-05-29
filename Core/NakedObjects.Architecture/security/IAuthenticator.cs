// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Security {
    public interface IAuthenticator {
        bool IsValid(IAuthenticationRequest request);

        bool CanAuthenticate(IAuthenticationRequest request);
    }

    // Copyright (c) Naked Objects Group Ltd.
}