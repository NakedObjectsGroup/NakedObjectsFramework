// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Security.Principal;
using System.Threading;
using NakedObjects.Architecture.Security;
using NakedObjects.Core.Context;

namespace NakedObjects.Core.Security {
    public class WindowsAuthenticationManager : IAuthenticationManager {
        #region IAuthenticationManager Members

        public string[] LogonNames {
            get { return new[] {WindowsIdentity.GetCurrent().Name}; }
        }

        public ISession Authenticate() {
            return CreateSessionWithCurrentIdentity();
        }

        public void CloseSession(ISession session) {
            NakedObjectsContext.CloseSession();
        }

        public void Init() {
            // do nothing
        }

        #endregion

        private static ISession CreateSessionWithCurrentIdentity() {
            WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();

            if (windowsIdentity != null) {
                Thread.CurrentPrincipal = new WindowsPrincipal(windowsIdentity);
                return new WindowsSession(Thread.CurrentPrincipal);
            }

            return null;
        }
    }
}