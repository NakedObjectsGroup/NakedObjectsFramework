// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;
using NakedObjects.Security;

namespace NakedObjects.Reflector.FacetFactory;

public sealed class AuthorizeAnnotationFacetFactory : DomainObjectFacetFactoryProcessor, IAnnotationBasedFacetFactory {
    private readonly ILogger<AuthorizeAnnotationFacetFactory> logger;

    public AuthorizeAnnotationFacetFactory(IFacetFactoryOrder<AuthorizeAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.PropertiesCollectionsAndActions) =>
        logger = loggerFactory.CreateLogger<AuthorizeAnnotationFacetFactory>();

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) => metamodel;

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var declaringType = method.DeclaringType;
#pragma warning disable CS0618 // Type or member is obsolete
        var classAttribute = declaringType?.GetCustomAttribute<AuthorizeActionAttribute>();
        var methodAttribute = method.GetCustomAttribute<AuthorizeActionAttribute>();
#pragma warning restore CS0618 // Type or member is obsolete

        if (classAttribute is not null && methodAttribute is not null) {
            logger.LogWarning($"Class and method level AuthorizeAttributes applied to class {declaringType.FullName} - ignoring attribute on method {method.Name}");
        }

        Create(classAttribute ?? methodAttribute, specification);
        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var declaringType = property.DeclaringType;
#pragma warning disable CS0618 // Type or member is obsolete
        var classAttribute = declaringType?.GetCustomAttribute<AuthorizePropertyAttribute>();
        var propertyAttribute = property.GetCustomAttribute<AuthorizePropertyAttribute>();
#pragma warning restore CS0618 // Type or member is obsolete

        if (classAttribute is not null && propertyAttribute is not null) {
            var declaringTypeName = declaringType.FullName;

            logger.LogWarning($"Class and property level AuthorizeAttributes applied to class {declaringTypeName} - ignoring attribute on property {property.Name}");
        }

        Create(classAttribute ?? propertyAttribute, specification);
        return metamodel;
    }

#pragma warning disable CS0618 // Type or member is obsolete
    private static void Create(AuthorizePropertyAttribute attribute, ISpecificationBuilder holder) {
        if (attribute is not null) {
            if (attribute.ViewRoles is not null || attribute.ViewUsers is not null) {
                FacetUtils.AddFacet(new AuthorizationHideForSessionFacetAnnotation(attribute.ViewRoles, attribute.ViewUsers), holder);
            }

            if (attribute.EditRoles is not null || attribute.EditUsers is not null) {
                FacetUtils.AddFacet(new AuthorizationDisableForSessionFacetAnnotation(attribute.EditRoles, attribute.EditUsers), holder);
            }
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete

#pragma warning disable CS0618 // Type or member is obsolete
    private static void Create(AuthorizeActionAttribute attribute, ISpecificationBuilder holder) {
        if (attribute is not null) {
            if (attribute.Roles is not null || attribute.Users is not null) {
                FacetUtils.AddFacet(new AuthorizationHideForSessionFacetAnnotation(attribute.Roles, attribute.Users), holder);
                FacetUtils.AddFacet(new AuthorizationDisableForSessionFacetAnnotation(attribute.Roles, attribute.Users), holder);
            }
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}