// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Security;

namespace NakedObjects.Core.Security {
    public class WindowsAuthenticatorInstaller : IAuthenticatorInstaller {
        #region IAuthenticatorInstaller Members

        public string Name {
            get { return "Windows"; }
        }


        public IAuthenticationManager CreateAuthenticationManager() {
            return new WindowsAuthenticationManager();
        }

        #endregion
    }
}