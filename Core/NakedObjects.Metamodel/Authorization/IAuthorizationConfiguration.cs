using System;
using System.Collections.Generic;
using NakedObjects.Security;

namespace NakedObjects.Meta.Authorization {
    public interface IAuthorizationConfiguration {
        Type DefaultAuthorizer { get; set; }
        Dictionary<string, Type> NamespaceAuthorizers { get; set; }
        Dictionary<Type, Type> TypeAuthorizers { get; set; }
        void SetNameSpaceAuthorizers(params INamespaceAuthorizer[] namespaceAuthorizers);
        void SetTypeAuthorizers(params object[] typeAuthorizers);
    }
}