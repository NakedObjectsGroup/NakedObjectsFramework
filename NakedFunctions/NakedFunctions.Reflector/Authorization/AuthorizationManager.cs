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
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Authorization;
using NakedFunctions.Reflector.Component;
using NakedFunctions.Security;

namespace NakedFunctions.Reflector.Authorization {
    [Serializable]
    public sealed class AuthorizationManager : IAuthorizationManager, IFacetDecorator {
        private readonly Type defaultAuthorizer;
        private readonly ImmutableDictionary<Type, Func<object, IPrincipal, object, string, bool>> isEditableDelegates;
        private readonly ImmutableDictionary<Type, Func<object, IPrincipal, object, string, bool>> isVisibleDelegates;
        private readonly ImmutableDictionary<string, Type> namespaceAuthorizers = ImmutableDictionary<string, Type>.Empty;
        private readonly ImmutableDictionary<string, Type> typeAuthorizers = ImmutableDictionary<string, Type>.Empty;

        private static object CreateAuthorizer(Type type, ILifecycleManager lifecycleManager) => lifecycleManager.CreateNonAdaptedInjectedObject(type);

        private object GetAuthorizer(INakedObjectAdapter target, ILifecycleManager lifecycleManager) {
            //Look for exact-fit TypeAuthorizer
            // order here as ImmutableDictionary not ordered
            var fullyQualifiedOfTarget = target.Spec.FullName;
            var authorizer = typeAuthorizers.Where(ta => ta.Key == fullyQualifiedOfTarget).Select(ta => ta.Value).FirstOrDefault() ??
                             namespaceAuthorizers.OrderByDescending(x => x.Key.Length).Where(x => fullyQualifiedOfTarget.StartsWith(x.Key)).Select(x => x.Value).FirstOrDefault() ??
                             defaultAuthorizer;

            return CreateAuthorizer(authorizer, lifecycleManager);
        }

        #region IAuthorizationManager Members

        public bool IsVisible(INakedObjectsFramework framework, INakedObjectAdapter target, IIdentifier identifier) {
            var authorizer = GetAuthorizer(target, framework.LifecycleManager);
            var authType = authorizer.GetType();

            if (typeof(INamespaceAuthorizer).IsAssignableFrom(authType)) {
                var nameAuth = (INamespaceAuthorizer) authorizer;
                return nameAuth.IsVisible(target.Object, identifier.MemberName, new FunctionalContext());
            }

            //Must be an ITypeAuthorizer, including default authorizer (ITypeAuthorizer<object>)
            return isVisibleDelegates[authType](authorizer, framework.Session.Principal, target.GetDomainObject(), identifier.MemberName);
        }

        public bool IsEditable(INakedObjectsFramework framework, INakedObjectAdapter target, IIdentifier identifier) {
            var authorizer = GetAuthorizer(target, framework.LifecycleManager);
            var authType = authorizer.GetType();

            if (typeof(INamespaceAuthorizer).IsAssignableFrom(authType)) {
                var nameAuth = (INamespaceAuthorizer) authorizer;
                return nameAuth.IsEditable(target.Object, identifier.MemberName, new FunctionalContext() );
            }

            return isEditableDelegates[authType](authorizer, framework.Session.Principal, target.GetDomainObject(), identifier.MemberName);
        }

        #endregion

        #region Constructors

        public AuthorizationManager(IAuthorizationConfiguration authorizationConfiguration, ILogger<AuthorizationManager> logger) {
            defaultAuthorizer = authorizationConfiguration.DefaultAuthorizer;
            if (defaultAuthorizer == null) {
                throw new InitialisationException(logger.LogAndReturn("Default Authorizer cannot be null"));
            }

            var isVisibleDict = new Dictionary<Type, Func<object, IPrincipal, object, string, bool>> {
                {defaultAuthorizer, DelegateUtils.CreateTypeAuthorizerDelegate(defaultAuthorizer.GetMethod("IsVisible"))}
            };

            var isEditableDict = new Dictionary<Type, Func<object, IPrincipal, object, string, bool>> {
                {defaultAuthorizer, DelegateUtils.CreateTypeAuthorizerDelegate(defaultAuthorizer.GetMethod("IsEditable"))}
            };

            if (authorizationConfiguration.NamespaceAuthorizers.Any()) {
                namespaceAuthorizers = authorizationConfiguration.NamespaceAuthorizers.ToImmutableDictionary();
            }

            if (authorizationConfiguration.TypeAuthorizers.Any()) {
                if (authorizationConfiguration.TypeAuthorizers.Values.Any(t => typeof(ITypeAuthorizer<object>).IsAssignableFrom(t))) {
                    throw new InitialisationException(logger.LogAndReturn("Only Default Authorizer can be ITypeAuthorizer<object>"));
                }

                typeAuthorizers = authorizationConfiguration.TypeAuthorizers.ToImmutableDictionary();
                isVisibleDelegates = isVisibleDict.Union(authorizationConfiguration.TypeAuthorizers.Values.ToDictionary(type => type, type => DelegateUtils.CreateTypeAuthorizerDelegate(type.GetMethod("IsVisible")))).ToImmutableDictionary();
                isEditableDelegates = isEditableDict.Union(authorizationConfiguration.TypeAuthorizers.Values.ToDictionary(type => type, type => DelegateUtils.CreateTypeAuthorizerDelegate(type.GetMethod("IsEditable")))).ToImmutableDictionary();
            }
            else {
                // default authorizer must be the only TypeAuthorizer
                isVisibleDelegates = isVisibleDict.ToImmutableDictionary();
                isEditableDelegates = isEditableDict.ToImmutableDictionary();
            }
        }

        public IFacet Decorate(IFacet facet, ISpecification holder) {
            var facetType = facet.FacetType;
            var specification = facet.Specification;
            var identifier = holder.Identifier;

            if (facetType == typeof(IHideForSessionFacet)) {
                return new AuthorizationHideForSessionFacet(identifier, this, specification);
            }

            if (facetType == typeof(IDisableForSessionFacet)) {
                return new AuthorizationDisableForSessionFacet(identifier, this, specification);
            }

            return facet;
        }

        public Type[] ForFacetTypes { get; } = {typeof(IHideForSessionFacet), typeof(IDisableForSessionFacet)};

        #endregion
    }
}