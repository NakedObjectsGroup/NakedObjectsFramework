// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Principal;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Security;

namespace NakedObjects.Meta.Authorization {
    [Serializable]
    public class AuthorizationManager : IAuthorizationManager, IFacetDecorator {
        private readonly Type defaultAuthorizer;
        private readonly Type[] forFacetTypes = {typeof (IHideForSessionFacet), typeof (IDisableForSessionFacet)};
        private readonly ImmutableDictionary<string, Type> namespaceAuthorizers = ImmutableDictionary<string, Type>.Empty;
        private readonly ImmutableDictionary<string, Type> typeAuthorizers = ImmutableDictionary<string, Type>.Empty;
        private readonly ImmutableDictionary<Type, Func<object, IPrincipal, object, string, bool>> isVisibleDelegates = ImmutableDictionary<Type, Func<object, IPrincipal, object, string, bool>>.Empty;
        private readonly ImmutableDictionary<Type, Func<object, IPrincipal, object, string, bool>> isEditableDelegates = ImmutableDictionary<Type, Func<object, IPrincipal, object, string, bool>>.Empty;


        #region IAuthorizationManager Members

        public bool IsVisible(ISession session, ILifecycleManager lifecycleManager, INakedObject target, IIdentifier identifier) {
            object authorizer = GetAuthorizer(target, lifecycleManager);
            Type authType = authorizer.GetType();

            if (authType.IsAssignableFrom(typeof (INamespaceAuthorizer))) {
                var nameAuth = (INamespaceAuthorizer) authorizer;
                return nameAuth.IsVisible(session.Principal, target.Object, identifier.MemberName);
            }
            return isVisibleDelegates[authType](authorizer, session.Principal, target.GetDomainObject(), identifier.MemberName);
        }

        public bool IsEditable(ISession session, ILifecycleManager lifecycleManager, INakedObject target, IIdentifier identifier) {
            object authorizer = GetAuthorizer(target, lifecycleManager);
            Type authType = authorizer.GetType();

            if (authType.IsAssignableFrom(typeof (INamespaceAuthorizer))) {
                var nameAuth = (INamespaceAuthorizer) authorizer;
                return nameAuth.IsEditable(session.Principal, target.Object, identifier.MemberName);
            }
            return isEditableDelegates[authType](authorizer, session.Principal, target.GetDomainObject(), identifier.MemberName);
        }

        #endregion

        private object CreateAuthorizer(Type type, ILifecycleManager lifecycleManager) {
            return lifecycleManager.CreateNonAdaptedInjectedObject(type);
        }

        private object GetAuthorizer(INakedObject target, ILifecycleManager lifecycleManager) {
            Assert.AssertNotNull(target);

            //Look for exact-fit TypeAuthorizer
            string fullyQualifiedOfTarget = target.Spec.FullName;
            Type authorizer = typeAuthorizers.
                Where(ta => ta.Key == fullyQualifiedOfTarget).
                Select(ta => ta.Value).
                FirstOrDefault() ??
                              namespaceAuthorizers.
                                  Where(x => fullyQualifiedOfTarget.StartsWith(x.Key)).
                                  OrderByDescending(x => x.Key.Length).
                                  Select(x => x.Value).
                                  FirstOrDefault() ??
                              defaultAuthorizer;

            return CreateAuthorizer(authorizer, lifecycleManager);
        }

        #region Constructors

        public AuthorizationManager(IAuthorizationConfiguration authorizationConfiguration) {
            defaultAuthorizer = authorizationConfiguration.DefaultAuthorizer;
            if (defaultAuthorizer == null) throw new InitialisationException("Default Authorizer cannot be null");
            if (authorizationConfiguration.NamespaceAuthorizers.Any()) {
                namespaceAuthorizers = authorizationConfiguration.NamespaceAuthorizers.ToImmutableDictionary();
            }
            if (authorizationConfiguration.TypeAuthorizers.Any()) {
                typeAuthorizers = authorizationConfiguration.TypeAuthorizers.ToImmutableDictionary();
                isVisibleDelegates = authorizationConfiguration.TypeAuthorizers.Values.ToDictionary(type => type, type => DelegateUtils.CreateTypeAuthorizerDelegate(type.GetMethod("IsVisible"))).ToImmutableDictionary();
                isEditableDelegates = authorizationConfiguration.TypeAuthorizers.Values.ToDictionary(type => type, type => DelegateUtils.CreateTypeAuthorizerDelegate(type.GetMethod("IsEditable"))).ToImmutableDictionary();
            }
        }

        public virtual IFacet Decorate(IFacet facet, ISpecification holder) {
            Type facetType = facet.FacetType;
            ISpecification specification = facet.Specification;
            IIdentifier identifier = holder.Identifier;

            if (facetType == typeof (IHideForSessionFacet)) {
                return new AuthorizationHideForSessionFacet(identifier, this, specification);
            }

            if (facetType == typeof (IDisableForSessionFacet)) {
                return new AuthorizationDisableForSessionFacet(identifier, this, specification);
            }
            return facet;
        }

        public virtual Type[] ForFacetTypes {
            get { return forFacetTypes; }
        }

        #endregion
    }
}