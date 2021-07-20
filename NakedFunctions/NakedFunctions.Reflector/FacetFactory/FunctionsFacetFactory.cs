// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;
using NakedFramework.ParallelReflector.FacetFactory;
using NakedFramework.ParallelReflector.Utils;
using NakedFunctions.Reflector.Facet;

namespace NakedFunctions.Reflector.FacetFactory {
    /// <summary>
    ///     Sets up all the <see cref="IFacet" />s for an action in a single shot
    /// </summary>
    public sealed class FunctionsFacetFactory : FunctionalFacetFactoryProcessor, IMethodPrefixBasedFacetFactory, IMethodIdentifyingFacetFactory {
        private static readonly string[] FixedPrefixes = Array.Empty<string>();

        private readonly ILogger<FunctionsFacetFactory> logger;

        public FunctionsFacetFactory(IFacetFactoryOrder<FunctionsFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.ActionsAndActionParameters) =>
            logger = loggerFactory.CreateLogger<FunctionsFacetFactory>();

        public string[] Prefixes => FixedPrefixes;

        // separate methods to reproduce old reflector behaviour
        private static bool IsParameterCollection(Type type) =>
            type is not null && (
                CollectionUtils.IsGenericEnumerable(type) ||
                type.IsArray ||
                type == typeof(string) ||
                CollectionUtils.IsCollectionButNotArray(type));

        private static bool IsCollection(Type type) =>
            type is not null && (
                CollectionUtils.IsGenericEnumerable(type) ||
                type.IsArray ||
                type == typeof(string) ||
                CollectionUtils.IsCollectionButNotArray(type) ||
                IsCollection(type.BaseType) ||
                type.GetInterfaces().Where(i => i.IsPublic).Any(IsCollection));

        /// <summary>
        ///     Must be called after the <c>CheckForXxxPrefix</c> methods.
        /// </summary>
        private static void DefaultNamedFacet(ICollection<IFacet> actionFacets, string name, ISpecification action) => actionFacets.Add(new NamedFacetInferred(name, action));

        #region IMethodIdentifyingFacetFactory Members

        private static bool IsUnsupportedSystemType(IReflector reflector, Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) => FasterTypeUtils.IsSystem(type) && !reflector.FindSpecification(type, metamodel);

        private static (ITypeSpecBuilder, Type, IImmutableDictionary<string, ITypeSpecBuilder>) LoadReturnSpecs(Type returnType, IImmutableDictionary<string, ITypeSpecBuilder> metamodel, IReflector reflector, MethodInfo actionMethod) {
            ITypeSpecBuilder onType = null;

            if (FacetUtils.IsTuple(returnType)) {
                var genericTypes = returnType.GetGenericArguments();

                if (genericTypes.Length == 0) {
                    throw new ReflectionException($"Cannot reflect empty tuple on {actionMethod.DeclaringType}.{actionMethod.Name}");
                }

                // count down so final result is first parameter
                for (var index = genericTypes.Length - 1; index >= 0; index--) {
                    var t = genericTypes[index];
                    (onType, returnType, metamodel) = LoadReturnSpecs(t, metamodel, reflector, actionMethod);
                }
            }
            else if (returnType.IsAssignableTo(typeof(IContext))) {
                (onType, metamodel) = reflector.LoadSpecification(typeof(void), metamodel);
            }
            else if (!IsUnsupportedSystemType(reflector, returnType, metamodel)) {
                (onType, metamodel) = reflector.LoadSpecification(returnType, metamodel);
            }
            else {
                throw new ReflectionException($"Cannot reflect return type {returnType} on {actionMethod.DeclaringType}.{actionMethod.Name}");
            }

            return (onType, returnType, metamodel);
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo actionMethod, ISpecificationBuilder action, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            // must be true
            if (!actionMethod.IsStatic) {
                throw new ReflectionException($"{actionMethod.DeclaringType}.{actionMethod.Name} must be static");
            }
            
            var capitalizedName = NameUtils.CapitalizeName(actionMethod.Name);

            var type = actionMethod.DeclaringType;
            var facets = new List<IFacet>();
            ITypeSpecBuilder onType;
            ITypeSpecBuilder returnSpec;
            (onType, metamodel) = reflector.LoadSpecification(type, metamodel);

            Type returnType;
            (returnSpec, returnType, metamodel) = LoadReturnSpecs(actionMethod.ReturnType, metamodel, reflector, actionMethod);

            if (!(returnSpec is IObjectSpecImmutable)) {
                throw new ReflectionException($"{returnSpec.Identifier} must be Object spec");
            }

            ITypeSpecImmutable elementSpec = null;
            var isQueryable = CollectionUtils.IsQueryable(returnType);
            if (returnSpec is IObjectSpecBuilder && IsCollection(returnType)) {
                var elementType = CollectionUtils.ElementType(returnType);
                (elementSpec, metamodel) = reflector.LoadSpecification(elementType, metamodel);
                if (elementSpec is not IObjectSpecImmutable) {
                    throw new ReflectionException($"{elementSpec.Identifier} must be Object spec");
                }
            }

            var invokeFacet = new ActionInvocationFacetViaStaticMethod(actionMethod, onType, (IObjectSpecImmutable) returnSpec, (IObjectSpecImmutable) elementSpec,
                                                                       action, isQueryable, LoggerFactory.CreateLogger<ActionInvocationFacetViaStaticMethod>());

            facets.Add(invokeFacet);

            DefaultNamedFacet(facets, actionMethod.Name, action); // must be called after the checkForXxxPrefix methods

            MethodHelpers.AddHideForSessionFacetNone(facets, action);
            MethodHelpers.AddDisableForSessionFacetNone(facets, action);

            facets.Add(new StaticFunctionFacet(action));

            FacetUtils.AddFacets(facets);

            return metamodel;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var parameter = method.GetParameters()[paramNum];
            var facets = new List<IFacet>();

            if (parameter.ParameterType.IsGenericType && parameter.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                facets.Add(new NullableFacetAlways(holder));
            }

            ITypeSpecBuilder returnSpec;
            (returnSpec, metamodel) = reflector.LoadSpecification(parameter.ParameterType, metamodel);

            if (!(returnSpec is IObjectSpecImmutable)) {
                throw new ReflectionException($"{returnSpec.Identifier} must be Object spec");
            }

            if (IsParameterCollection(parameter.ParameterType)) {
                var elementType = CollectionUtils.ElementType(parameter.ParameterType);
                ITypeSpecImmutable elementSpec;
                (elementSpec, metamodel) = reflector.LoadSpecification(elementType, metamodel);
                if (!(elementSpec is IObjectSpecImmutable)) {
                    throw new ReflectionException($"{elementSpec.Identifier} must be Object spec");
                }

                facets.Add(new ElementTypeFacet(holder, elementType, (IObjectSpecImmutable) elementSpec));
            }

            FacetUtils.AddFacets(facets);
            return metamodel;
        }

        private static bool IsStatic(Type type) => type.IsAbstract && type.IsSealed;

        public IList<MethodInfo> FindActions(IList<MethodInfo> candidates, IClassStrategy classStrategy) {
            var ignored = candidates.Where(methodInfo => classStrategy.IsIgnored(methodInfo) ||
                                                         methodInfo.ReturnType == typeof(void) ||
                                                         !methodInfo.IsStatic ||
                                                         !IsStatic(methodInfo.DeclaringType)).ToArray();

            foreach (var method in ignored.Where(m => m.ReturnType == typeof(void))) {
                if (!FasterTypeUtils.IsSystemOrNaked(method.DeclaringType)) {
                    logger.LogWarning($"Ignored as returns void {method.DeclaringType}.{method.Name}");
                }
            }

            return candidates.Except(ignored).ToArray();
        }

        #endregion
    }
}