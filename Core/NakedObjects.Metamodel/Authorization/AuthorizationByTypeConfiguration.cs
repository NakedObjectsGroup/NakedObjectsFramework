// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Core;

namespace NakedObjects.Meta.Authorization {
    public class AuthorizationByTypeConfiguration : IAuthorizationByTypeConfiguration {
        private IDictionary<Type, Type> typeAuthorizers = new Dictionary<Type, Type>();

        #region IAuthorizationByTypeConfiguration Members

        public Type DefaultAuthorizer { get; set; }

        public IDictionary<Type, Type> TypeAuthorizers {
            get { return typeAuthorizers; }
            set { typeAuthorizers = value; }
        }

        public void SetTypeAuthorizers(object defaultAuthorizer, params object[] typeAuthorizers) {
            ValidateAuthorizer(defaultAuthorizer);
            DefaultAuthorizer = defaultAuthorizer.GetType();

            foreach (object typeAuth in typeAuthorizers) {
                var domainType = ValidateAuthorizer(typeAuth);
                TypeAuthorizers.Add(domainType, typeAuth.GetType());
            }
        }

        private static Type ValidateAuthorizer(object typeAuth) {
            Type authInt = typeAuth.GetType().GetInterface("ITypeAuthorizer`1");
            if (authInt == null || authInt.GetGenericArguments().First().IsAbstract) {
                throw new InitialisationException("Attempting to specify a typeAuthorizer that does not implement ITypeAuthorizer<T>, where T is concrete");
            }
            return authInt.GetGenericArguments().First();
        }

        #endregion
    }
}