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
using NakedObjects.Util;

namespace NakedObjects.Meta.Authorization {
    [Serializable]
    public class AuthorizationManager : IAuthorizationManager, IFacetDecorator {
        private readonly Type defaultAuthorizer;
        private readonly Type[] forFacetTypes = {typeof (IHideForSessionFacet), typeof (IDisableForSessionFacet)};
        private readonly ImmutableDictionary<string, Type> namespaceAuthorizers = ImmutableDictionary<string, Type>.Empty;
        private readonly ImmutableDictionary<string, Type> typeAuthorizers = ImmutableDictionary<string, Type>.Empty;

        #region Constructors

        public AuthorizationManager(IAuthorizationConfiguration authorizationConfiguration) {
            defaultAuthorizer = authorizationConfiguration.DefaultAuthorizer;
            if (defaultAuthorizer == null) throw new InitialisationException("Default Authorizer cannot be null");
            if (authorizationConfiguration.NamespaceAuthorizers.Any()) {
                namespaceAuthorizers = authorizationConfiguration.NamespaceAuthorizers.ToImmutableDictionary();
            }
            if (authorizationConfiguration.TypeAuthorizers.Any()) {
                typeAuthorizers = authorizationConfiguration.TypeAuthorizers.ToImmutableDictionary();
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

        #region IAuthorizationManager Members

        public bool IsVisible(ISession session, ILifecycleManager lifecycleManager, IMetamodelManager manager, INakedObject target, IIdentifier identifier) {
            object authorizer = GetAuthorizer(target, lifecycleManager, manager);

            if (authorizer.GetType().IsAssignableFrom(typeof(INamespaceAuthorizer))) {
                ITypeAuthorizer<object> nameAuth = (ITypeAuthorizer<object>)authorizer;
                return nameAuth.IsVisible(session.Principal, target.Object, identifier.MemberName);
            } else {
                return (bool)ExecuteOnTypeAuthorizer(session, target, identifier, "IsVisible", authorizer);
            }
        }

        public bool IsEditable(ISession session, ILifecycleManager lifecycleManager, IMetamodelManager manager, INakedObject target, IIdentifier identifier) {
            object authorizer = GetAuthorizer(target, lifecycleManager, manager);

             if (authorizer.GetType().IsAssignableFrom(typeof(INamespaceAuthorizer)))  {
                ITypeAuthorizer<object> nameAuth = (ITypeAuthorizer<object>) authorizer;
                return nameAuth.IsEditable(session.Principal, target.Object, identifier.MemberName);
            } else {
                return (bool)ExecuteOnTypeAuthorizer(session, target, identifier, "IsEditable", authorizer);
            }
        }

        #endregion

        private static object ExecuteOnTypeAuthorizer(ISession session, INakedObject target, IIdentifier identifier, string toInvoke, object authorizer) {
            return authorizer.GetType().GetMethod(toInvoke).Invoke(authorizer, new[] { session.Principal, target.Object, identifier.MemberName });
        }

        private object GetAuthorizer(INakedObject target, ILifecycleManager lifecycleManager, 
            IMetamodelManager manager) {
                    
            Assert.AssertNotNull(target);
            Type authorizer = null;

            //Look for exact-fit TypeAuthorizer
            string fullyQualifiedOfTarget = target.Spec.FullName;
            authorizer = typeAuthorizers.
                Where(ta => ta.Key == fullyQualifiedOfTarget).
                Select(ta => ta.Value).
                FirstOrDefault();

            if (authorizer == null) {//If not find best namespace authorizer


                authorizer = namespaceAuthorizers.
                    Where(x => fullyQualifiedOfTarget.StartsWith(x.Key)).
                    OrderByDescending(x => x.Key.Length).
                    Select(x => x.Value).
                    FirstOrDefault();

                //Last resort: use the default
                if (authorizer == null) {
                    authorizer = defaultAuthorizer;
                }
            }
            return lifecycleManager.CreateInstance(manager.GetSpecification(authorizer)).GetDomainObject();
        }
    }
}