// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Security;
using NakedObjects.Core.NakedObjectsSystem;

namespace NakedObjects.Core.Security {
    public interface IAuthenticatorInstaller : IInstaller {
        IAuthenticationManager CreateAuthenticationManager();
    }
}