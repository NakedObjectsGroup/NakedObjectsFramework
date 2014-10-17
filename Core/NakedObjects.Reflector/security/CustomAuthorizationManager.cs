// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
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

        public IMetamodel Metamodel { get; set; }

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
            Type domainType = TypeUtils.GetType(target.Spec.FullName).GetProxiedType();
            object authorizer;
            typeAuthorizerMap.TryGetValue(domainType, out authorizer);
            return authorizer == null ? null : CreateAuthorizer(authorizer, persistor);
        }

        private object GetDefaultAuthorizor(ILifecycleManager persistor) {
            return CreateAuthorizer(defaultAuthorizer, persistor);
        }

        private object CreateAuthorizer(object authorizer, ILifecycleManager persistor) {
            // return persistor.CreateObject(Metamodel.GetSpecification(authorizer.GetType())); 
            //return Reflector.LoadSpecification(authorizer.GetType()).CreateObject(persistor);
            throw new NotImplementedException();
        }

        //TODO:  Change return type to INamespaceAuthorizer when TypeAuthorization has been obsoleted.
        private object GetNamespaceAuthorizerFor(INakedObject target, ILifecycleManager persistor) {
            Assert.AssertNotNull(target);
            string fullyQualifiedOfTarget = target.Spec.FullName;
            INamespaceAuthorizer authorizer = namespaceAuthorizers.
                Where(x => fullyQualifiedOfTarget.StartsWith(x.NamespaceToAuthorize)).
                OrderByDescending(x => x.NamespaceToAuthorize.Length).
                FirstOrDefault();

            return authorizer == null ? null : CreateAuthorizer(authorizer, persistor);
        }
    }
}