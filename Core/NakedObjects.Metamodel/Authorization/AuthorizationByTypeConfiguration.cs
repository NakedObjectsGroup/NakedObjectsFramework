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
using NakedObjects.Security;

namespace NakedObjects.Meta.Authorization {
    //Add type authorizers individually via AddTypeAuthorizer, or create the whole dictionary
    //and set the TypeAuthorizers property.
    public class AuthorizationByTypeConfiguration<TDefault> 
        : IAuthorizationByTypeConfiguration 
        where TDefault : ITypeAuthorizer<object>{

        public AuthorizationByTypeConfiguration() {
            DefaultAuthorizer = typeof(TDefault);
            TypeAuthorizers = new Dictionary<Type, Type>();
        }

        #region IAuthorizationByTypeConfiguration Members

        public Type DefaultAuthorizer { get; private set; }

        public IDictionary<Type, Type> TypeAuthorizers { get; set; }

        public void AddTypeAuthorizer<TDomain, TAuth>() 
            where TDomain : new()
            where TAuth : ITypeAuthorizer<TDomain> {
                TypeAuthorizers.Add(typeof(TDomain), typeof(TAuth));
        }
        #endregion
    }
}