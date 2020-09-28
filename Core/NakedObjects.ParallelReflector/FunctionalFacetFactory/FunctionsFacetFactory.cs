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
using NakedFunctions.Meta.Facet;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;
using NakedObjects.ParallelReflect.FacetFactory;
using NakedObjects.Util;

namespace NakedObjects.ParallelReflect.FunctionalFacetFactory {
    /// <summary>
    ///     Sets up all the <see cref="IFacet" />s for an action in a single shot
    /// </summary>
    public sealed class FunctionsFacetFactory : MethodPrefixBasedFacetFactoryAbstract, IMethodIdentifyingFacetFactory {
        private static readonly string[] FixedPrefixes = { };

        private readonly ILogger<ActionMethodsFacetFactory> logger;

        public FunctionsFacetFactory(int numericOrder, ILoggerFactory loggerFactory)
            : base(numericOrder, loggerFactory, FeatureType.ActionsAndActionParameters, ReflectionType.Functional) =>
            logger = loggerFactory.CreateLogger<ActionMethodsFacetFactory>();

        public override string[] Prefixes => FixedPrefixes;

        private bool IsQueryOnly(MethodInfo method) =>
            method.GetCustomAttribute<IdempotentAttribute>() == null &&
            method.GetCustomAttribute<QueryOnlyAttribute>() != null;

        // separate methods to reproduce old reflector behaviour
        private bool IsParameterCollection(Type type) =>
            type != null && (
                CollectionUtils.IsGenericEnumerable(type) ||
                type.IsArray ||
                type == typeof(string) ||
                CollectionUtils.IsCollectionButNotArray(type));

        private bool IsCollection(Type type) =>
            type != null && (
                CollectionUtils.IsGenericEnumerable(type) ||
                type.IsArray ||
                type == typeof(string) ||
                CollectionUtils.IsCollectionButNotArray(type) ||
                IsCollection(type.BaseType) ||
                type.GetInterfaces().Where(i => i.IsPublic).Any(IsCollection));

        private bool ParametersAreSupported(MethodInfo method, IClassStrategy classStrategy) {
            foreach (var parameterInfo in method.GetParameters()) {
                if (!classStrategy.IsTypeToBeIntrospected(parameterInfo.ParameterType)) {
                    // log if not a System or NOF type
                    if (!TypeUtils.IsSystem(method.DeclaringType) && !TypeUtils.IsNakedObjects(method.DeclaringType)) {
                        logger.LogWarning("Ignoring method: {method.DeclaringType}.{method.Name} because parameter '{parameterInfo.Name}' is of type {parameterInfo.ParameterType}");
                    }

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Must be called after the <c>CheckForXxxPrefix</c> methods.
        /// </summary>
        private static void DefaultNamedFacet(ICollection<IFacet> actionFacets, string name, ISpecification action) => actionFacets.Add(new NamedFacetInferred(name, action));

        #region IMethodIdentifyingFacetFactory Members

        private (ITypeSpecBuilder, Type, IImmutableDictionary<string, ITypeSpecBuilder>) LoadReturnSpecs(Type returnType, IImmutableDictionary<string, ITypeSpecBuilder> metamodel, IReflector reflector, MethodInfo actionMethod) {
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
            else {
                (onType, metamodel) = reflector.LoadSpecification(returnType, metamodel);
            }

            return (onType, returnType, metamodel);
        }


        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo actionMethod, IMethodRemover methodRemover, ISpecificationBuilder action, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
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
            var isQueryable = IsQueryOnly(actionMethod) || CollectionUtils.IsQueryable(returnType);
            if (returnSpec is IObjectSpecBuilder && IsCollection(returnType)) {
                var elementType = CollectionUtils.ElementType(returnType);
                (elementSpec, metamodel) = reflector.LoadSpecification(elementType, metamodel);
                if (!(elementSpec is IObjectSpecImmutable)) {
                    throw new ReflectionException($"{elementSpec.Identifier} must be Object spec");
                }
            }


            RemoveMethod(methodRemover, actionMethod);

            var invokeFacet = new ActionInvocationFacetViaStaticMethod(actionMethod, onType, (IObjectSpecImmutable) returnSpec, (IObjectSpecImmutable) elementSpec,
                                                                       action, isQueryable, LoggerFactory.CreateLogger<ActionInvocationFacetViaStaticMethod>());

            facets.Add(invokeFacet);

            DefaultNamedFacet(facets, actionMethod.Name, action); // must be called after the checkForXxxPrefix methods

            AddHideForSessionFacetNone(facets, action);
            AddDisableForSessionFacetNone(facets, action);

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
            return candidates.Where(methodInfo => methodInfo.GetCustomAttribute<NakedObjectsIgnoreAttribute>() == null &&
                                                  methodInfo.IsStatic &&
                                                  IsStatic(methodInfo.DeclaringType)).ToArray();
        }

        #endregion
    }
}