// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Security.Principal;

namespace NakedObjects.Architecture.Security {
    /// <summary>
    ///     The representation within the system of an authenticated user
    /// </summary>
    public interface ISession {
        /// <summary>
        ///     The name of the authenticated user; for display purposes only
        /// </summary>
        string UserName { get; }

        bool IsAuthenticated { get; }

        IPrincipal Principal { get; }
    }

    // Copyright (c) Naked Objects Group Ltd.
}