// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Security;

namespace NakedObjects.Meta.Authorization {
    //Add namespace authorizers individually via AddNamespaceAuthorizer, or create the whole dictionary
    //and set the NamespaceAuthorizers property.
    public class AuthorizationByNamespaceConfiguration<TDefault> 
        : IAuthorizationByNamespaceConfiguration 
        where TDefault : ITypeAuthorizer<object> {

        public AuthorizationByNamespaceConfiguration() {
            DefaultAuthorizer = typeof(TDefault);
            NamespaceAuthorizers = new Dictionary<string, Type>();
        }

        #region IAuthorizationByNamespaceConfiguration Members

        public Type DefaultAuthorizer { get; private set; }

        public IDictionary<string, Type> NamespaceAuthorizers { get; set; }

        public void AddNamespaceAuthorizer<TAuth>(string namespaceCovered)
            where TAuth : ITypeAuthorizer<object> {
            NamespaceAuthorizers.Add(namespaceCovered, typeof(TAuth));
        }

        public void AddTypeAuthorizer<TDomain, TAuth>()
            where TDomain : new()
            where TAuth : ITypeAuthorizer<TDomain> {
            string fullyQualifiedName = typeof(TDomain).FullName;
            NamespaceAuthorizers.Add(fullyQualifiedName, typeof(TAuth));
        }

        #endregion
    }
}