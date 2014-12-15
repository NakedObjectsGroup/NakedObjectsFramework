// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Security;

namespace NakedObjects.Meta.Authorization {
    [Serializable]
    public class AuthorizationByNamespaceManager : IAuthorizationManager, IFacetDecorator {
        private readonly Type defaultAuthorizer;
        private readonly Type[] forFacetTypes = {typeof (IHideForSessionFacet), typeof (IDisableForSessionFacet)};
        private readonly ImmutableDictionary<string, Type> namespaceAuthorizers = ImmutableDictionary<string, Type>.Empty;

        #region Constructors

        public AuthorizationByNamespaceManager(IAuthorizationByNamespaceConfiguration authorizationConfiguration) {
            defaultAuthorizer = authorizationConfiguration.DefaultAuthorizer;
            if (defaultAuthorizer == null) throw new InitialisationException("Default Authorizer cannot be null");

            if (authorizationConfiguration.NamespaceAuthorizers.Any()) {
                namespaceAuthorizers = authorizationConfiguration.NamespaceAuthorizers.ToImmutableDictionary();
            }

            Validate();
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

        // validate all the passed in types to fail at reflection time as far as possible
        private void Validate() {
            if (!typeof(ITypeAuthorizer<object>).IsAssignableFrom(defaultAuthorizer)) {
                throw new InitialisationException(defaultAuthorizer.FullName + "is not an INamespaceAuthorizer");
            }

            if (namespaceAuthorizers.Any()) {
                namespaceAuthorizers.ForEach(kvp => {
                    if (!typeof (ITypeAuthorizer<object>).IsAssignableFrom(kvp.Value)) {
                        throw new InitialisationException(kvp.Value.FullName + "is not an INamespaceAuthorizer");
                    }
                });
            }
        }

        #endregion

        #region IAuthorizationManager Members

        public bool IsEditable(ISession session, ILifecycleManager lifecycleManager, IMetamodelManager manager, INakedObject target, IIdentifier identifier) {
            Assert.AssertNotNull(target);

            ITypeAuthorizer<object> authorizer = GetNamespaceAuthorizerFor(target, lifecycleManager, manager) ?? GetDefaultAuthorizer(lifecycleManager, manager);
            return authorizer.IsEditable(session.Principal, target.Object, identifier.MemberName);
        }

        public bool IsVisible(ISession session, ILifecycleManager lifecycleManager, IMetamodelManager manager, INakedObject target, IIdentifier identifier) {
            Assert.AssertNotNull(target);

            ITypeAuthorizer<object> authorizer = GetNamespaceAuthorizerFor(target, lifecycleManager, manager) ?? GetDefaultAuthorizer(lifecycleManager, manager);
            return authorizer.IsVisible(session.Principal, target.Object, identifier.MemberName);
        }

        #endregion

        private ITypeAuthorizer<object> GetDefaultAuthorizer(ILifecycleManager lifecycleManager, IMetamodelManager manager) {
            return CreateAuthorizer(defaultAuthorizer, lifecycleManager, manager);
        }

        private ITypeAuthorizer<object> CreateAuthorizer(Type authorizer, ILifecycleManager lifecycleManager, IMetamodelManager manager) {
            return lifecycleManager.CreateInstance(manager.GetSpecification(authorizer)).GetDomainObject<ITypeAuthorizer<object>>();
        }

        private ITypeAuthorizer<object> GetNamespaceAuthorizerFor(INakedObject target, ILifecycleManager lifecycleManager, IMetamodelManager manager) {
            Assert.AssertNotNull(target);
            string fullyQualifiedOfTarget = target.Spec.FullName;
            Type authorizer = namespaceAuthorizers.
                Where(x => fullyQualifiedOfTarget.StartsWith(x.Key)).
                OrderByDescending(x => x.Key.Length).
                Select(x => x.Value).
                FirstOrDefault();

            return authorizer != null ? CreateAuthorizer(authorizer, lifecycleManager, manager) : null;
        }
    }
}