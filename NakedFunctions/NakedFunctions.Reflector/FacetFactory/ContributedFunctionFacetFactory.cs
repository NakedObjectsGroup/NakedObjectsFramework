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
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using NakedFunctions.Meta.Facet;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Utils;

namespace NakedFunctions.Reflector.FacetFactory {
    /// <summary>
    /// </summary>
    public sealed class ContributedFunctionFacetFactory : FunctionalFacetFactoryProcessor {
        private readonly ILogger<ContributedFunctionFacetFactory> logger;

        public ContributedFunctionFacetFactory(IFacetFactoryOrder<ContributedFunctionFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.Actions) =>
            logger = loggerFactory.CreateLogger<ContributedFunctionFacetFactory>();

        private static bool IsContributedToObjectOrCollection(MethodInfo member) => member.IsDefined(typeof(ExtensionAttribute), false);

        private static Type GetContributeeType(MethodInfo member) => IsContributedToObjectOrCollection(member) ? member.GetParameters().First().ParameterType : member.DeclaringType;

        private static bool IsParseable(Type type) => type.IsValueType;

        private static bool IsCollection(Type type) =>
            type is not null && (
                CollectionUtils.IsGenericEnumerable(type) ||
                type.IsArray ||
                CollectionUtils.IsCollectionButNotArray(type) ||
                IsCollection(type.BaseType) ||
                type.GetInterfaces().Where(i => i.IsPublic).Any(IsCollection));

        private IImmutableDictionary<string, ITypeSpecBuilder> AddCollectionContributedAction(IReflector reflector, MethodInfo member, Type parameterType, ContributedFunctionFacet facet, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (!CollectionUtils.IsGenericQueryable(parameterType)) {
                logger.LogWarning($"Query Contributed Function ignored as it is added to a collection parameter type other than IQueryable: {member.Name}");
            }
            else {
                if (IsCollection(member.ReturnType)) {
                    logger.LogWarning($"Query Contributed Function ignored as it returns a collection: {member.Name}");
                }
                else {
                    var elementType = parameterType.GetGenericArguments()[0];
                    IObjectSpecBuilder type;
                    (type, metamodel) = reflector.LoadSpecification<IObjectSpecBuilder>(elementType, metamodel);
                    facet.AddCollectionContributee(type);
                    FacetUtils.AddFacet(facet);
                }
            }

            return metamodel;
        }

        private static IImmutableDictionary<string, ITypeSpecBuilder> AddLocalCollectionContributedAction(IReflector reflector, ParameterInfo p, ContributedFunctionFacet facet, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var elementType = p.ParameterType.GetGenericArguments()[0];
            IObjectSpecBuilder type;
            (type, metamodel) = reflector.LoadSpecification<IObjectSpecBuilder>(elementType, metamodel);
            facet.AddLocalCollectionContributee(type, p.Name);
            return metamodel;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            // all functions are contributed to first parameter or if menu, itself

            if (!method.IsDefined(typeof(DisplayAsPropertyAttribute), false)) {
                var parameterType = GetContributeeType(method);
                ITypeSpecImmutable type;
                var facet = new ContributedFunctionFacet(specification, IsContributedToObjectOrCollection(method));

                if (IsParseable(parameterType)) {
                    logger.LogWarning($"Query Contributed Function ignored as it is added to a collection of value types : {method.Name}");
                }
                else if (IsCollection(parameterType)) {
                    metamodel = AddCollectionContributedAction(reflector, method, parameterType, facet, metamodel);
                }
                else {
                    (type, metamodel) = reflector.LoadSpecification(parameterType, metamodel);
                    facet.AddContributee(type);
                    FacetUtils.AddFacet(facet);
                }
            }

            return metamodel;
        }
    }
}