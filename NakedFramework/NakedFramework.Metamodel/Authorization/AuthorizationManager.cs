// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Principal;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Framework;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Security;

namespace NakedFramework.Metamodel.Authorization {
    [Serializable]
    public sealed class AuthorizationManager : AbstractAuthorizationManager {

        private readonly ImmutableDictionary<Type, Func<object, IPrincipal, object, string, bool>> isEditableDelegates;
        private readonly ImmutableDictionary<Type, Func<object, IPrincipal, object, string, bool>> isVisibleDelegates;

        public AuthorizationManager(IAuthorizationConfiguration authorizationConfiguration, ILogger<AuthorizationManager> logger) : base(authorizationConfiguration, logger) {
            var isVisibleDict = new Dictionary<Type, Func<object, IPrincipal, object, string, bool>> {
                { defaultAuthorizer, DelegateUtils.CreateObjectTypeAuthorizerDelegate(defaultAuthorizer.GetMethod("IsVisible")) }
            };

            var isEditableDict = new Dictionary<Type, Func<object, IPrincipal, object, string, bool>> {
                { defaultAuthorizer, DelegateUtils.CreateObjectTypeAuthorizerDelegate(defaultAuthorizer.GetMethod("IsEditable")) }
            };

            if (typeAuthorizers.Any()) {
                if (typeAuthorizers.Values.Any(t => typeof(ITypeAuthorizer<object>).IsAssignableFrom(t))) {
                    throw new InitialisationException(logger.LogAndReturn("Only Default Authorizer can be ITypeAuthorizer<object>"));
                }

                isVisibleDelegates = isVisibleDict.Union(typeAuthorizers.Values.ToDictionary(type => type, type => DelegateUtils.CreateObjectTypeAuthorizerDelegate(type.GetMethod("IsVisible")))).ToImmutableDictionary();
                isEditableDelegates = isEditableDict.Union(typeAuthorizers.Values.ToDictionary(type => type, type => DelegateUtils.CreateObjectTypeAuthorizerDelegate(type.GetMethod("IsEditable")))).ToImmutableDictionary();
            }
            else {
                // default authorizer must be the only TypeAuthorizer
                isVisibleDelegates = isVisibleDict.ToImmutableDictionary();
                isEditableDelegates = isEditableDict.ToImmutableDictionary();
            }
        }

        protected override object CreateAuthorizer(Type type, ILifecycleManager lifecycleManager) => lifecycleManager.CreateNonAdaptedInjectedObject(type);

        public override bool IsVisible(INakedObjectsFramework framework, INakedObjectAdapter target, IIdentifier identifier) {
            var authorizer = GetAuthorizer(target, framework.LifecycleManager);
            var authType = authorizer.GetType();

            if (typeof(INamespaceAuthorizer).IsAssignableFrom(authType)) {
                var nameAuth = (INamespaceAuthorizer)authorizer;
                return nameAuth.IsVisible(framework.Session.Principal, target.Object, identifier.MemberName);
            }

            //Must be an ITypeAuthorizer, including default authorizer (ITypeAuthorizer<object>)
            return isVisibleDelegates[authType](authorizer, framework.Session.Principal, target.GetDomainObject(), identifier.MemberName);
        }

        public override bool IsEditable(INakedObjectsFramework framework, INakedObjectAdapter target, IIdentifier identifier) {
            var authorizer = GetAuthorizer(target, framework.LifecycleManager);
            var authType = authorizer.GetType();

            if (typeof(INamespaceAuthorizer).IsAssignableFrom(authType)) {
                var nameAuth = (INamespaceAuthorizer)authorizer;
                return nameAuth.IsEditable(framework.Session.Principal, target.Object, identifier.MemberName);
            }

            return isEditableDelegates[authType](authorizer, framework.Session.Principal, target.GetDomainObject(), identifier.MemberName);
        }
    }
}