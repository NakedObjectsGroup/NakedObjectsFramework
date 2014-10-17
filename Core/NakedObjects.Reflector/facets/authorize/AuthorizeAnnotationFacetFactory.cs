// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Disable;
using NakedObjects.Architecture.Facets.Hide;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Security;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Authorize {
    public class AuthorizeAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        private static readonly ILog Log = LogManager.GetLogger(typeof (AuthorizeAnnotationFacetFactory));

        public AuthorizeAnnotationFacetFactory(INakedObjectReflector reflector)
            : base(reflector, FeatureType.PropertiesCollectionsAndActions) {}


        public override bool Process(Type type, IMethodRemover methodRemover, ISpecification specification) {
            return false;
        }


        public override bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecification specification) {
            var classAttribute = method.DeclaringType.GetCustomAttributeByReflection<AuthorizeActionAttribute>();
            var methodAttribute = AttributeUtils.GetCustomAttribute<AuthorizeActionAttribute>(method);

            if (classAttribute != null && methodAttribute != null) {
                Log.WarnFormat("Class and method level AuthorizeAttributes applied to class {0} - ignoring attribute on method {1}", method.DeclaringType.FullName, method.Name);
            }

            return Create(classAttribute ?? methodAttribute, specification);
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, ISpecification specification) {
            var classAttribute = property.DeclaringType.GetCustomAttributeByReflection<AuthorizePropertyAttribute>();
            var propertyAttribute = AttributeUtils.GetCustomAttribute<AuthorizePropertyAttribute>(property);

            if (classAttribute != null && propertyAttribute != null) {
                Log.WarnFormat("Class and property level AuthorizeAttributes applied to class {0} - ignoring attribute on property {1}", property.DeclaringType.FullName, property.Name);
            }

            return Create(classAttribute ?? propertyAttribute, specification);
        }

        private static bool Create(AuthorizePropertyAttribute attribute, ISpecification holder) {
            bool added = false;

            if (attribute != null) {
                if (attribute.ViewRoles != null || attribute.ViewUsers != null) {
                    var facet = new SecurityHideForSessionFacet(attribute.ViewRoles, attribute.ViewUsers, holder);
                    added = FacetUtils.AddFacet(facet);
                }

                if (attribute.EditRoles != null || attribute.EditUsers != null) {
                    var facet = new SecurityDisableForSessionFacet(attribute.EditRoles, attribute.EditUsers, holder);
                    added |= FacetUtils.AddFacet(facet);
                }
            }

            return added;
        }

        private static bool Create(AuthorizeActionAttribute attribute, ISpecification holder) {
            bool added = false;

            if (attribute != null) {
                if (attribute.Roles != null || attribute.Users != null) {
                    IFacet facet = new SecurityHideForSessionFacet(attribute.Roles, attribute.Users, holder);
                    added = FacetUtils.AddFacet(facet);
                    facet = new SecurityDisableForSessionFacet(attribute.Roles, attribute.Users, holder);
                    added |= FacetUtils.AddFacet(facet);
                }
            }

            return added;
        }

        private static string[] SplitOnComma(string toSplit) {
            if (string.IsNullOrEmpty(toSplit)) {
                return new string[] {};
            }
            return toSplit.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
        }

        private static bool IsAllowed(ISession session, string[] roles, string[] users) {
            if (roles.Any(role => session.Principal.IsInRole(role))) {
                return true;
            }

            if (users.Any(user => session.Principal.Identity.Name == user)) {
                return true;
            }

            return false;
        }

        #region Nested type: SecurityDisableForSessionFacet

        public class SecurityDisableForSessionFacet : DisableForSessionFacetAbstract {
            private readonly string[] roles;
            private readonly string[] users;

            public SecurityDisableForSessionFacet(string roles,
                                                  string users,
                                                  ISpecification holder)
                : base(holder) {
                this.roles = SplitOnComma(roles);
                this.users = SplitOnComma(users);
            }

            public override string DisabledReason(ISession session, INakedObject target, ILifecycleManager persistor) {
                return IsAllowed(session, roles, users) ? null : "Not authorized to edit";
            }
        }

        #endregion

        #region Nested type: SecurityHideForSessionFacet

        public class SecurityHideForSessionFacet : HideForSessionFacetAbstract {
            private readonly string[] roles;
            private readonly string[] users;

            public SecurityHideForSessionFacet(string roles,
                                               string users,
                                               ISpecification holder)
                : base(holder) {
                this.roles = SplitOnComma(roles);
                this.users = SplitOnComma(users);
            }

            public override string HiddenReason(ISession session, INakedObject target, ILifecycleManager persistor) {
                return IsAllowed(session, roles, users) ? null : "Not authorized to view";
            }
        }

        #endregion
    }
}