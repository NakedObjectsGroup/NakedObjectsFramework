// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.Reflection;
using Common.Logging;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;
using NakedObjects.Security;

namespace NakedObjects.ParallelReflect.FacetFactory {
    public sealed class AuthorizeAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        private readonly ILogger<AuthorizeAnnotationFacetFactory> logger;

        public AuthorizeAnnotationFacetFactory(int numericOrder, ILoggerFactory loggerFactory)
            : base(numericOrder, loggerFactory, FeatureType.PropertiesCollectionsAndActions) =>
            logger = loggerFactory.CreateLogger<AuthorizeAnnotationFacetFactory>();

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) => metamodel;

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var declaringType = method.DeclaringType;
            var classAttribute = declaringType?.GetCustomAttribute<AuthorizeActionAttribute>();
            var methodAttribute = method.GetCustomAttribute<AuthorizeActionAttribute>();

            if (classAttribute != null && methodAttribute != null) {
                var declaringTypeName = declaringType.FullName;
                logger.LogWarning($"Class and method level AuthorizeAttributes applied to class {declaringTypeName} - ignoring attribute on method {method.Name}");
            }

            Create(classAttribute ?? methodAttribute, specification);
            return metamodel;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var declaringType = property.DeclaringType;
            var classAttribute = declaringType?.GetCustomAttribute<AuthorizePropertyAttribute>();
            var propertyAttribute = property.GetCustomAttribute<AuthorizePropertyAttribute>();

            if (classAttribute != null && propertyAttribute != null) {
                var declaringTypeName = declaringType.FullName;

                logger.LogWarning($"Class and property level AuthorizeAttributes applied to class {declaringTypeName} - ignoring attribute on property {property.Name}");
            }

            Create(classAttribute ?? propertyAttribute, specification);
            return metamodel;
        }

        private static void Create(AuthorizePropertyAttribute attribute, ISpecification holder) {
            if (attribute != null) {
                if (attribute.ViewRoles != null || attribute.ViewUsers != null) {
                    FacetUtils.AddFacet(new AuthorizationHideForSessionFacet(attribute.ViewRoles, attribute.ViewUsers, holder));
                }

                if (attribute.EditRoles != null || attribute.EditUsers != null) {
                    FacetUtils.AddFacet(new AuthorizationDisableForSessionFacet(attribute.EditRoles, attribute.EditUsers, holder));
                }
            }
        }

        private static void Create(AuthorizeActionAttribute attribute, ISpecification holder) {
            if (attribute != null) {
                if (attribute.Roles != null || attribute.Users != null) {
                    FacetUtils.AddFacet(new AuthorizationHideForSessionFacet(attribute.Roles, attribute.Users, holder));
                    FacetUtils.AddFacet(new AuthorizationDisableForSessionFacet(attribute.Roles, attribute.Users, holder));
                }
            }
        }
    }
}