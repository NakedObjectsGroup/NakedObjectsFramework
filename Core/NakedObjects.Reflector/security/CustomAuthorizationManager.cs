using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Util;
using NakedObjects.Util;

namespace NakedObjects.Security {
    public class CustomAuthorizationManager : IAuthorizationManager {
        private readonly ITypeAuthorizer<object> defaultAuthorizer;
        private readonly INamespaceAuthorizer[] namespaceAuthorizers = {};
        private readonly Dictionary<Type, object> typeAuthorizerMap = new Dictionary<Type, object>();

        #region Constructors

        public CustomAuthorizationManager(ITypeAuthorizer<object> defaultAuthorizer) {
            if (defaultAuthorizer == null) throw new InitialisationException("Default Authorizer cannot be null");

            this.defaultAuthorizer = defaultAuthorizer;
        }

        public CustomAuthorizationManager(ITypeAuthorizer<object> defaultAuthorizer, params INamespaceAuthorizer[] namespaceAuthorizers)
            : this(defaultAuthorizer) {
            this.namespaceAuthorizers = namespaceAuthorizers;
        }

        public INakedObjectReflector Reflector { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="defaultAuthorizer">This will be used unless the object type exactly matches one of the typeAuthorizers</param>
        /// <param name="typeAuthorizers">Each authorizer must implement ITypeAuthorizer of T, where T is a concrete domain type</param>
        public CustomAuthorizationManager(ITypeAuthorizer<object> defaultAuthorizer, params object[] typeAuthorizers)
            : this(defaultAuthorizer) {
            foreach (object typeAuth in typeAuthorizers) {
                Type authInt = typeAuth.GetType().GetInterface("ITypeAuthorizer`1");
                if (authInt != null && !(authInt.GetGenericArguments()[0]).IsAbstract) {
                    Type domainType = authInt.GetGenericArguments()[0];
                    typeAuthorizerMap.Add(domainType, typeAuth);
                }
                else {
                    throw new InitialisationException("Attempting to specify a typeAuthorizer that does not implement ITypeAuthorizer<T>, where T is concrete");
                }
            }
        }

        #endregion

        #region IAuthorizationManager Members

        public bool IsEditable(ISession session, ILifecycleManager persistor, INakedObject target, IIdentifier identifier) {
            return IsEditableOrVisible(session, persistor, target, identifier, "IsEditable");
        }

        public bool IsVisible(ISession session, ILifecycleManager persistor, INakedObject target, IIdentifier identifier) {
            return IsEditableOrVisible(session, persistor, target, identifier, "IsVisible");
        }

        public void UpdateAuthorizationCache(INakedObject nakedObject) {
            //Does nothing
        }

        #endregion

        private bool IsEditableOrVisible(ISession session, ILifecycleManager persistor, INakedObject target, IIdentifier identifier, string toInvoke) {
            Assert.AssertNotNull(target);

            object authorizer = GetNamespaceAuthorizerFor(target, persistor) ?? GetTypeAuthorizerFor(target, persistor) ?? GetDefaultAuthorizor(persistor);
            return (bool) authorizer.GetType().GetMethod(toInvoke).Invoke(authorizer, new[] {session.Principal, target.Object, identifier.MemberName});
        }

        private object GetTypeAuthorizerFor(INakedObject target, ILifecycleManager persistor) {
            Assert.AssertNotNull(target);
            Type domainType = TypeUtils.GetType(target.Specification.FullName).GetProxiedType();
            object authorizer;
            typeAuthorizerMap.TryGetValue(domainType, out authorizer);
            return authorizer == null ? null : CreateAuthorizer(authorizer, persistor);
        }

        private object GetDefaultAuthorizor(ILifecycleManager persistor) {
            return CreateAuthorizer(defaultAuthorizer, persistor);
        }

        private object CreateAuthorizer(object authorizer, ILifecycleManager persistor) {
            return persistor.CreateObject(Reflector.LoadSpecification(authorizer.GetType())); 
            //return Reflector.LoadSpecification(authorizer.GetType()).CreateObject(persistor);
        }

        //TODO:  Change return type to INamespaceAuthorizer when TypeAuthorization has been obsoleted.
        private object GetNamespaceAuthorizerFor(INakedObject target, ILifecycleManager persistor) {
            Assert.AssertNotNull(target);
            string fullyQualifiedOfTarget = target.Specification.FullName;
            INamespaceAuthorizer authorizer = namespaceAuthorizers.
                Where(x => fullyQualifiedOfTarget.StartsWith(x.NamespaceToAuthorize)).
                OrderByDescending(x => x.NamespaceToAuthorize.Length).
                FirstOrDefault();

            return authorizer == null ? null : CreateAuthorizer(authorizer, persistor);
        }
    }
}