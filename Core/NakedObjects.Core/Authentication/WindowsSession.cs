// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.IO;
using System.Security.Principal;
using NakedObjects.Architecture.Component;

namespace NakedObjects.Core.Authentication {
    public class WindowsSession : ISession {
        public WindowsSession(IPrincipal principal) {
            Principal = principal ?? new EmptyPrincipal();
        }

        #region ISession Members

        public IPrincipal Principal { get; protected set; }

        public string UserName {
            get { return Path.GetFileName(Principal.Identity.Name); }
        }

        public bool IsAuthenticated {
            get { return Principal.Identity.IsAuthenticated; }
        }

        #endregion

        #region Nested type: EmptyIdentity

        private class EmptyIdentity : IIdentity {
            public EmptyIdentity() {
                Name = "";
                AuthenticationType = "";
                IsAuthenticated = false;
            }

            #region IIdentity Members

            public string Name { get; private set; }
            public string AuthenticationType { get; private set; }
            public bool IsAuthenticated { get; private set; }

            #endregion
        }

        #endregion

        #region Nested type: EmptyPrincipal

        private class EmptyPrincipal : IPrincipal {
            public EmptyPrincipal() {
                Identity = new EmptyIdentity();
            }

            #region IPrincipal Members

            public bool IsInRole(string role) {
                return false;
            }

            public IIdentity Identity { get; private set; }

            #endregion
        }

        #endregion
    }
}