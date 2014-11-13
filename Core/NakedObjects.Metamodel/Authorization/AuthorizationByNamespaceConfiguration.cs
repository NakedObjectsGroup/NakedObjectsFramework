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
    public class AuthorizationByNamespaceConfiguration : IAuthorizationByNamespaceConfiguration {
        private IDictionary<string, Type> namespaceAuthorizers = new Dictionary<string, Type>();

        #region IAuthorizationByNamespaceConfiguration Members

        public Type DefaultAuthorizer { get; set; }

        public IDictionary<string, Type> NamespaceAuthorizers {
            get { return namespaceAuthorizers; }
            set { namespaceAuthorizers = value; }
        }

        public void SetNameSpaceAuthorizers(INamespaceAuthorizer defaultAuthorizer, params INamespaceAuthorizer[] namespaceAuthorizers) {
            DefaultAuthorizer = defaultAuthorizer.GetType();
            NamespaceAuthorizers = namespaceAuthorizers.ToDictionary(na => na.NamespaceToAuthorize, na => na.GetType());
        }

        #endregion
    }
}