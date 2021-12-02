// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Runtime.CompilerServices;
using System.Security.Principal;

[assembly: InternalsVisibleTo("NakedFunctions.Services.Test")]

namespace NakedFramework.Core.Authentication;

internal class EmptyPrincipal : IPrincipal {
    public EmptyPrincipal() => Identity = new EmptyIdentity();

    #region IPrincipal Members

    public bool IsInRole(string role) => false;

    public IIdentity Identity { get; }

    #endregion
}

internal class EmptyIdentity : IIdentity {
    public EmptyIdentity() {
        Name = "";
        AuthenticationType = "";
        IsAuthenticated = false;
    }

    #region IIdentity Members

    public string Name { get; }
    public string AuthenticationType { get; }
    public bool IsAuthenticated { get; }

    #endregion
}