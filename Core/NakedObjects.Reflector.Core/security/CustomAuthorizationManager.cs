using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Context;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Util;
using NakedObjects.Util;

namespace NakedObjects.Security {
    public class CustomAuthorizationManager : IAuthorizationManager {
        private readonly ITypeAuthorizer<object> defaultAuthorizer;
        private readonly INamespaceAuthorizer[] namespaceAuthorizers = {};
        private readonly Dictionary<Type, object> typeAuthorizerMap = new Dictionary<Type, object>();

        private bool isInitialised;

        #region Constructors

        public CustomAuthorizationManager(ITypeAuthorizer<object> defaultAuthorizer) {
            if (defaultAuthorizer == null) throw new InitialisationException("Default Authorizer cannot be null");
            this.defaultAuthorizer = defaultAuthorizer;
        }

        public CustomAuthorizationManager(ITypeAuthorizer<object> defaultAuthorizer, params INamespaceAuthorizer[] namespaceAuthorizers)
            : this(defaultAuthorizer) {
            this.namespaceAuthorizers = namespaceAuthorizers;
        }

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

        private void Inject() {
            object[] services = NakedObjectsContext.ObjectPersistor.GetServices().Select(no => no.Object).ToArray();
            IContainerInjector injector = NakedObjectsContext.Reflector.CreateContainerInjector(services);
            injector.InitDomainObject(defaultAuthorizer);

            namespaceAuthorizers.ForEach(injector.InitDomainObject);
            typeAuthorizerMap.ForEach(kvp => injector.InitDomainObject(kvp.Value));
        }

        #endregion

        #region IAuthorizationManager Members

        public bool IsEditable(ISession session, INakedObject target, IIdentifier identifier) {
            return IsEditableOrVisible(session, target, identifier, "IsEditable");
        }

        public bool IsVisible(ISession session, INakedObject target, IIdentifier identifier) {
            return IsEditableOrVisible(session, target, identifier, "IsVisible");
        }

        public void UpdateAuthorizationCache(INakedObject nakedObject) {
            //Does nothing
        }

        public void Init() {
            isInitialised = true;
            Inject();
            defaultAuthorizer.Init();
            namespaceAuthorizers.OfType<IRequiresSetup>().ForEach(na => na.Init());
            typeAuthorizerMap.Select(kvp => kvp.Value).OfType<IRequiresSetup>().ForEach(ta => ta.Init());
        }

        public void Shutdown() {
            //does nothing
        }

        private bool IsEditableOrVisible(ISession session, INakedObject target, IIdentifier identifier, string toInvoke) {
            Assert.AssertNotNull(target);
            // initialise on first use
            if (!isInitialised) {
                Init();
            }
            object authorizer = GetNamespaceAuthorizerFor(target) ?? GetTypeAuthorizerFor(target) ?? defaultAuthorizer;
            return (bool) authorizer.GetType().GetMethod(toInvoke).Invoke(authorizer, new[] {session.Principal, target.Object, identifier.MemberName});
        }

        #endregion

        private object GetTypeAuthorizerFor(INakedObject target) {
            Assert.AssertNotNull(target);
            Type domainType = TypeUtils.GetType(target.Specification.FullName).GetProxiedType();
            object authorizer;
            typeAuthorizerMap.TryGetValue(domainType, out authorizer);
            return authorizer;
        }

        //TODO:  Change return type to INamespaceAuthorizer when TypeAuthorization has been obsoleted.
        private object GetNamespaceAuthorizerFor(INakedObject target) {
            Assert.AssertNotNull(target);
            string fullyQualifiedOfTarget = target.Specification.FullName;
            return namespaceAuthorizers.
                Where(x => fullyQualifiedOfTarget.StartsWith(x.NamespaceToAuthorize)).
                OrderByDescending(x => x.NamespaceToAuthorize.Length).
                FirstOrDefault();
        }
    }

    [Obsolete("Spelling has changed from 's' to 'z'")]
    public class CustomAuthorisationManager : CustomAuthorizationManager {
        public CustomAuthorisationManager(ITypeAuthorizer<object> defaultAuthorizer, params object[] typeAuthorizers)
            : base(defaultAuthorizer, typeAuthorizers) {}
    }
}