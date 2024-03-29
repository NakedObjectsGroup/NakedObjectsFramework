// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;

namespace NakedObjects.Reflector.FacetFactory;

/// <summary>
///     Creates an <see cref="IContributedActionIntegrationFacet" /> based on the presence of an
///     <see cref="ContributedActionAttribute" /> annotation
/// </summary>
public sealed class ContributedActionAnnotationFacetFactory : DomainObjectFacetFactoryProcessor, IAnnotationBasedFacetFactory {
    private readonly ILogger<ContributedActionAnnotationFacetFactory> logger;

    public ContributedActionAnnotationFacetFactory(IFacetFactoryOrder<ContributedActionAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.Actions) =>
        logger = loggerFactory.CreateLogger<ContributedActionAnnotationFacetFactory>();

    private static bool IsParseable(Type type) => type.IsValueType;

    private static bool IsCollection(Type type) =>
        type is not null && (
            CollectionUtils.IsGenericEnumerable(type) ||
            type.IsArray ||
            CollectionUtils.IsCollectionButNotArray(type) ||
            IsCollection(type.BaseType) ||
            type.GetInterfaces().Where(i => i.IsPublic).Any(IsCollection));

    private IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo member, ISpecificationBuilder holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var allParams = member.GetParameters();
        var paramsWithAttribute = allParams.Select(p => (p, p.GetCustomAttribute<ContributedActionAttribute>())).Where(p => p.Item2 is not null).ToArray();
        var isDisplayAsProperty = member.IsDefined(typeof(DisplayAsPropertyAttribute), false);
        if (isDisplayAsProperty || !paramsWithAttribute.Any()) {
            return metamodel; //Nothing to do
        }

        var facet = new ContributedActionIntegrationFacet();
        foreach (var (p, attribute) in paramsWithAttribute) {
            var parameterType = p.ParameterType;
            (var type, metamodel) = reflector.LoadSpecification<IObjectSpecBuilder>(parameterType, metamodel);

            if (type is not null) {
                if (IsParseable(parameterType)) {
                    logger.LogWarning($"ContributedAction attribute added to a value parameter type: {member.Name}");
                }
                else if (IsCollection(parameterType)) {
                    (var parent, metamodel) = reflector.LoadSpecification<IObjectSpecImmutable>(member.DeclaringType, metamodel);
                    metamodel = parent is IObjectSpecBuilder
                        ? AddLocalCollectionContributedAction(reflector, p, holder, metamodel)
                        : AddCollectionContributedAction(reflector, member, parameterType, p, attribute, facet, metamodel);
                }
                else {
                    facet.AddObjectContributee(type, attribute.SubMenu, attribute.Id);
                }
            }
        }

        if (facet.IsContributed) {
            FacetUtils.AddFacet(facet, holder);
            FacetUtils.AddFacet(ContributedActionFacet.Instance, holder);
        }

        return metamodel;
    }

    private IImmutableDictionary<string, ITypeSpecBuilder> AddCollectionContributedAction(IReflector reflector, MethodInfo member, Type parameterType, ParameterInfo p, ContributedActionAttribute attribute, ContributedActionIntegrationFacet integrationFacet, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        if (!CollectionUtils.IsGenericQueryable(parameterType)) {
            logger.LogWarning($"ContributedAction attribute added to a collection parameter type other than IQueryable: {member.Name}");
        }
        else {
            var returnType = member.ReturnType;
            (_, metamodel) = reflector.LoadSpecification<IObjectSpecImmutable>(returnType, metamodel);
            if (IsCollection(returnType)) {
                logger.LogWarning($"ContributedAction attribute added to an action that returns a collection: {member.Name}");
            }
            else {
                var elementType = p.ParameterType.GetGenericArguments()[0];
                (var type, metamodel) = reflector.LoadSpecification<IObjectSpecBuilder>(elementType, metamodel);
                integrationFacet.AddCollectionContributee(type, attribute.SubMenu, attribute.Id);
            }
        }

        return metamodel;
    }

    private static IImmutableDictionary<string, ITypeSpecBuilder> AddLocalCollectionContributedAction(IReflector reflector, ParameterInfo p, ISpecificationBuilder holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var facet = new ContributedToLocalCollectionIntegrationFacet();
        var elementType = p.ParameterType.GetGenericArguments()[0];
        (var type, metamodel) = reflector.LoadSpecification<IObjectSpecBuilder>(elementType, metamodel);
        facet.AddLocalCollectionContributee(type, p.Name);
        FacetUtils.AddFacet(facet, holder);
        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) =>
        Process(reflector, method, specification, metamodel);
}