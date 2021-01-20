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
using NakedObjects;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Utils;
using NakedObjects.ParallelReflector.FacetFactory;
using NakedObjects.ParallelReflector.Utils;

namespace NakedFunctions.Reflector.FacetFactory {
    public sealed class ActionChoicesViaFunctionFacetFactory : FunctionalFacetFactoryProcessor, IMethodPrefixBasedFacetFactory, IMethodFilteringFacetFactory {
        private static readonly string[] FixedPrefixes = {
            RecognisedMethodsAndPrefixes.ParameterChoicesPrefix
        };

        private readonly ILogger<ActionChoicesViaFunctionFacetFactory> logger;

        public ActionChoicesViaFunctionFacetFactory(IFacetFactoryOrder<ActionChoicesViaFunctionFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.Actions) =>
            logger = loggerFactory.CreateLogger<ActionChoicesViaFunctionFacetFactory>();

        public string[] Prefixes => FixedPrefixes;

        private IImmutableDictionary<string, ITypeSpecBuilder> FindChoicesMethod(IReflector reflector, Type declaringType, string capitalizedName, Type[] paramTypes, IActionParameterSpecImmutable[] parameters, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            for (var i = 0; i < paramTypes.Length; i++) {
                var paramType = paramTypes[i];
                var isMultiple = false;

                if (CollectionUtils.IsGenericEnumerable(paramType)) {
                    paramType = paramType.GetGenericArguments().First();
                    isMultiple = true;
                }

                var returnType = typeof(IEnumerable<>).MakeGenericType(paramType);

                var methodToUse = FindChoicesMethod(declaringType, capitalizedName, i, returnType);

                if (methodToUse is not null) {
                    // add facets directly to parameters, not to actions
                    var parameterNamesAndTypes = new List<(string, IObjectSpecImmutable)>();

                    foreach (var parameterInfo in InjectUtils.FilterParms(methodToUse)) {
                        ITypeSpecBuilder typeSpecBuilder;
                        (typeSpecBuilder, metamodel) = reflector.LoadSpecification(parameterInfo.ParameterType, metamodel);
                        var name = parameterInfo.Name?.ToLower();
                        if (typeSpecBuilder is IObjectSpecBuilder objectSpec && name is not null) {
                            parameterNamesAndTypes.Add((name, objectSpec));
                        }
                        else {
                            logger.LogWarning($"Unexpected name: {name} or spec: {typeSpecBuilder}");
                        }
                    }

                    FacetUtils.AddFacet(new ActionChoicesFacetViaFunction(methodToUse, parameterNamesAndTypes.ToArray(), returnType, parameters[i], isMultiple));
                    MethodHelpers.AddOrAddToExecutedWhereFacet(methodToUse, parameters[i]);
                }
            }

            return metamodel;
        }

        private static bool Matches(MethodInfo methodInfo, string name, Type type, Type returnType) =>
            methodInfo.Name == name &&
            MatchReturnType(methodInfo.ReturnType, returnType);

        private static bool MatchReturnType(Type returnType, Type toMatch) => CollectionUtils.IsGenericEnumerable(returnType) && returnType.GenericTypeArguments.SequenceEqual(toMatch.GenericTypeArguments);

        private MethodInfo FindChoicesMethod(Type declaringType, string capitalizedName, int i, Type returnType) {
            var name = $"{RecognisedMethodsAndPrefixes.ParameterChoicesPrefix}{i}{capitalizedName}";
            var choicesMethod = declaringType.GetMethods().SingleOrDefault(methodInfo => Matches(methodInfo, name, declaringType, returnType));
            var nameMatches = declaringType.GetMethods().Where(mi => mi.Name == name && mi != choicesMethod);

            foreach (var methodInfo in nameMatches) {
                logger.LogWarning($"choices method found: {methodInfo.Name} not matching expected signature");
            }

            return choicesMethod;
        }

        #region IMethodFilteringFacetFactory Members

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo actionMethod, ISpecificationBuilder action, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var capitalizedName = NameUtils.CapitalizeName(actionMethod.Name);
            var declaringType = actionMethod.DeclaringType;
            var paramTypes = actionMethod.GetParameters().Select(p => p.ParameterType).ToArray();

            if (action is IActionSpecImmutable actionSpecImmutable) {
                var actionParameters = actionSpecImmutable.Parameters;
                metamodel = FindChoicesMethod(reflector, declaringType, capitalizedName, paramTypes, actionParameters, metamodel);
            }

            return metamodel;
        }

        public bool Filters(MethodInfo method, IClassStrategy classStrategy) => method.Name.StartsWith(RecognisedMethodsAndPrefixes.ParameterChoicesPrefix);

        #endregion
    }
}