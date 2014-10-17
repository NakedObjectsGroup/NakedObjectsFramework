// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.IO;
using System.Security.Principal;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Security;

namespace NakedObjects.Core.Security {
    public class WindowsSession : ISession {
        private class EmptyIdentity : IIdentity {
            public EmptyIdentity() {
                Name = "";
                AuthenticationType = "";
                IsAuthenticated = false;
            }

            public string Name { get; private set; }
            public string AuthenticationType { get; private set; }
            public bool IsAuthenticated { get; private set; }
        }

        private class EmptyPrincipal : IPrincipal {
            public EmptyPrincipal() {
                Identity = new EmptyIdentity();
            }

            public bool IsInRole(string role) {
                return false;
            }

            public IIdentity Identity { get; private set; }
        }

        public WindowsSession(IPrincipal principal) {
            Principal = principal ?? new EmptyPrincipal();
        }

        public IPrincipal Principal { get; protected set; }

        #region ISession Members

        public string UserName {
            // todo - might be better to concatenate domain and account - what does windows do ?
            get {
                return Path.GetFileName(Principal.Identity.Name);
            }
        }

        public bool IsAuthenticated {
            get { return Principal.Identity.IsAuthenticated; }
        }

        #endregion
    }
}