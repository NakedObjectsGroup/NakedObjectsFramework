// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedFramework.Metamodel.Authorization;
using NakedFunctions.Security;

namespace NakedFunctions.Reflector.Authorization; 

public class AuthorizationConfiguration<TDefault, TMainMenu>
    : IAuthorizationConfiguration
    where TDefault : ITypeAuthorizer<object>
    where TMainMenu : IMainMenuAuthorizer {
    public AuthorizationConfiguration() {
        DefaultAuthorizer = typeof(TDefault);
        NamespaceAuthorizers = new Dictionary<string, Type>();
        var stringQualifiedName = typeof(string).FullName;

        TypeAuthorizers = new Dictionary<string, Type>() { { stringQualifiedName, typeof(TMainMenu) } };
    }

    //The specified type authorizer will apply to the whole namespace specified
    public void AddNamespaceAuthorizer<TAuth>(string namespaceCovered)
        where TAuth : INamespaceAuthorizer =>
        NamespaceAuthorizers.Add(namespaceCovered, typeof(TAuth));

    //The specified type authorizer will apply only to the domain object type specified (not even sub-classes)
    public void AddTypeAuthorizer<TDomain, TAuth>()
        where TDomain : new()
        where TAuth : ITypeAuthorizer<TDomain> {
        var fullyQualifiedName = typeof(TDomain).FullName;
        TypeAuthorizers.Add(fullyQualifiedName, typeof(TAuth));
    }

    #region IAuthorizationConfiguration Members

    public Type DefaultAuthorizer { get; }
    public IDictionary<string, Type> NamespaceAuthorizers { get; }
    public IDictionary<string, Type> TypeAuthorizers { get; }

    #endregion
}