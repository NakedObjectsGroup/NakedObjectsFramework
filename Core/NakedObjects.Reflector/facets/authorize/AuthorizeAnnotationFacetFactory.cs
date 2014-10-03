// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Linq;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Disable;
using NakedObjects.Architecture.Facets.Hide;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Security;
using NakedObjects.Util;
using MethodInfo = System.Reflection.MethodInfo;
using PropertyInfo = System.Reflection.PropertyInfo;

namespace NakedObjects.Reflector.DotNet.Facets.Authorize {
    public class AuthorizeAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        private static readonly ILog Log = LogManager.GetLogger(typeof (AuthorizeAnnotationFacetFactory));

        public AuthorizeAnnotationFacetFactory(INakedObjectReflector reflector)
            :base(reflector, NakedObjectFeatureType.PropertiesCollectionsAndActions) { }


        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            return false;
        }


        public override bool Process(MethodInfo method, IMethodRemover methodRemover, IFacetHolder holder) {
            var classAttribute = method.DeclaringType.GetCustomAttributeByReflection<AuthorizeActionAttribute>();
            var methodAttribute = method.GetCustomAttribute<AuthorizeActionAttribute>();

            if (classAttribute != null && methodAttribute != null) {
                Log.WarnFormat("Class and method level AuthorizeAttributes applied to class {0} - ignoring attribute on method {1}", method.DeclaringType.FullName, method.Name);
            }

            return Create(classAttribute ?? methodAttribute, holder);
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, IFacetHolder holder) {
            var classAttribute = property.DeclaringType.GetCustomAttributeByReflection<AuthorizePropertyAttribute>();
            var propertyAttribute = property.GetCustomAttribute<AuthorizePropertyAttribute>();

            if (classAttribute != null && propertyAttribute != null) {
                Log.WarnFormat("Class and property level AuthorizeAttributes applied to class {0} - ignoring attribute on property {1}", property.DeclaringType.FullName, property.Name);
            }

            return Create(classAttribute ?? propertyAttribute, holder);
        }

        private static bool Create(AuthorizePropertyAttribute attribute, IFacetHolder holder) {
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

        private static bool Create(AuthorizeActionAttribute attribute, IFacetHolder holder) {
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
                                                  IFacetHolder holder)
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
                                               IFacetHolder holder)
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