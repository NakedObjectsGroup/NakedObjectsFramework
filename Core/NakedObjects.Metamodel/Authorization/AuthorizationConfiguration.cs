using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Security;

namespace NakedObjects.Meta.Authorization {
    public class AuthorizationConfiguration : IAuthorizationConfiguration {
        public Type DefaultAuthorizer { get; set; }
        public Dictionary<string, Type> NamespaceAuthorizers { get; set; }

        public Dictionary<Type, Type> TypeAuthorizers { get; set; }

        public void SetNameSpaceAuthorizers(params INamespaceAuthorizer[] namespaceAuthorizers) {
            NamespaceAuthorizers = namespaceAuthorizers.ToDictionary(na => na.NamespaceToAuthorize, na => na.GetType());
        }

        public void SetTypeAuthorizers(params object[] typeAuthorizers) {
            TypeAuthorizers = new Dictionary<Type, Type>();

            foreach (object typeAuth in typeAuthorizers) {
                Type authInt = typeAuth.GetType().GetInterface("ITypeAuthorizer`1");
                if (authInt != null && !(authInt.GetGenericArguments()[0]).IsAbstract) {
                    Type domainType = authInt.GetGenericArguments()[0];
                    TypeAuthorizers.Add(domainType, typeAuth.GetType());
                }
                else {
                    throw new InitialisationException("Attempting to specify a typeAuthorizer that does not implement ITypeAuthorizer<T>, where T is concrete");
                }
            }
        }
    }
}