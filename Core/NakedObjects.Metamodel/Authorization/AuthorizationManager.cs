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
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Util;
using NakedObjects.Reflect.Spec;
using NakedObjects.Util;

namespace NakedObjects.Meta.Authorization {
    [Serializable]
    public class AuthorizationManager : IAuthorizationManager, IFacetDecorator {
        private readonly Type defaultAuthorizer;
        private readonly Type[] forFacetTypes = {typeof (IHideForSessionFacet), typeof (IDisableForSessionFacet)};
        private readonly ImmutableDictionary<string, Type> namespaceAuthorizers = ImmutableDictionary<string, Type>.Empty;
        private readonly ImmutableDictionary<Type, Type> typeAuthorizers = ImmutableDictionary<Type, Type>.Empty;

        #region Constructors

        public AuthorizationManager(IAuthorizationConfiguration authorizationConfiguration) {
            defaultAuthorizer = authorizationConfiguration.DefaultAuthorizer;

            if (authorizationConfiguration.NamespaceAuthorizers.Any()) {
                namespaceAuthorizers = authorizationConfiguration.NamespaceAuthorizers.ToImmutableDictionary();
            }
            else {
                typeAuthorizers = authorizationConfiguration.TypeAuthorizers.ToImmutableDictionary();
            }

            if (defaultAuthorizer == null) throw new InitialisationException("Default Authorizer cannot be null");

            Validate();
        }

        // validate all the passed in types to fail at reflection time as far as possible
        private void Validate() {
            Assert.AssertTrue(defaultAuthorizer.FullName + "is not an IAuthorizer", typeof(IAuthorizer).IsAssignableFrom(defaultAuthorizer));
            if (namespaceAuthorizers.Any()) {             
                namespaceAuthorizers.ForEach(kvp => Assert.AssertTrue(kvp.Value.FullName + "is not an IAuthorizer", typeof(IAuthorizer).IsAssignableFrom(kvp.Value)));
            }
            // typeAuthorizers validated in config class
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

        #region IAuthorizationManager Members

        public bool IsEditable(ISession session, ILifecycleManager lifecycleManager, IMetamodelManager manager, INakedObject target, IIdentifier identifier) {
            return IsEditableOrVisible(session, lifecycleManager, manager, target, identifier, "IsEditable");
        }

        public bool IsVisible(ISession session, ILifecycleManager lifecycleManager, IMetamodelManager manager, INakedObject target, IIdentifier identifier) {
            return IsEditableOrVisible(session, lifecycleManager, manager, target, identifier, "IsVisible");
        }

        #endregion

        private bool IsEditableOrVisible(ISession session, ILifecycleManager lifecycleManager, IMetamodelManager manager, INakedObject target, IIdentifier identifier, string toInvoke) {
            Assert.AssertNotNull(target);

            object authorizer = GetNamespaceAuthorizerFor(target, lifecycleManager, manager) ?? GetTypeAuthorizerFor(target, lifecycleManager, manager) ?? GetDefaultAuthorizer(lifecycleManager, manager);
            return (bool) authorizer.GetType().GetMethod(toInvoke).Invoke(authorizer, new[] {session.Principal, target.Object, identifier.MemberName});
        }

        private object GetTypeAuthorizerFor(INakedObject target, ILifecycleManager lifecycleManager, IMetamodelManager manager) {
            Assert.AssertNotNull(target);
            Type domainType = TypeUtils.GetType(target.Spec.FullName).GetProxiedType();
            Type authorizer;
            typeAuthorizers.TryGetValue(domainType, out authorizer);
            return authorizer == null ? null : CreateAuthorizer(authorizer, lifecycleManager, manager);
        }

        private object GetDefaultAuthorizer(ILifecycleManager lifecycleManager, IMetamodelManager manager) {
            return CreateAuthorizer(defaultAuthorizer, lifecycleManager, manager);
        }

        private object CreateAuthorizer(Type authorizer, ILifecycleManager lifecycleManager, IMetamodelManager manager) {
            return lifecycleManager.CreateInstance(manager.GetSpecification(authorizer)).GetDomainObject<IAuthorizer>();
        }

        //TODO:  Change return type to INamespaceAuthorizer when TypeAuthorization has been obsoleted.
        private object GetNamespaceAuthorizerFor(INakedObject target, ILifecycleManager lifecycleManager, IMetamodelManager manager) {
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