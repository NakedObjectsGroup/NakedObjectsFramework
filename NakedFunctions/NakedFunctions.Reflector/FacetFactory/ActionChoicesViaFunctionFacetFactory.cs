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
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Utils;
using NakedFramework.ParallelReflector.FacetFactory;
using NakedFunctions.Reflector.Facet;
using NakedFunctions.Reflector.Utils;

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

        private IImmutableDictionary<string, ITypeSpecBuilder> FindChoicesMethod(IReflector reflector, Type declaringType, string capitalizedName, Type[] paramTypes, MethodInfo actionMethod, IActionParameterSpecImmutable[] parameters, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            for (var i = 0; i < paramTypes.Length; i++) {
                var paramType = paramTypes[i];
                var isMultiple = false;

                if (CollectionUtils.IsGenericEnumerable(paramType)) {
                    paramType = paramType.GetGenericArguments().First();
                    isMultiple = true;
                }

                var returnType = typeof(IEnumerable<>).MakeGenericType(paramType);
                var name = $"{RecognisedMethodsAndPrefixes.ParameterChoicesPrefix}{i}{capitalizedName}";
                bool Matcher(MethodInfo mi) => Matches(mi, name, returnType);

                var methodToUse = FactoryUtils.FindComplementaryMethod(declaringType, name, Matcher, logger);

                if (methodToUse is not null) {
                    // add facets directly to parameters, not to actions
                    var parameterNamesAndTypes = new List<(string, IObjectSpecImmutable)>();
                    var mismatchedParm = false;

                    foreach (var parameterInfo in InjectUtils.FilterParms(methodToUse)) {
                        ITypeSpecBuilder typeSpecBuilder;
                        (typeSpecBuilder, metamodel) = reflector.LoadSpecification(parameterInfo.ParameterType, metamodel);
                        var paramName = parameterInfo.Name?.ToLower();
                        if (typeSpecBuilder is IObjectSpecBuilder objectSpec && paramName is not null) {
                            parameterNamesAndTypes.Add((paramName, objectSpec));
                        }
                        else {
                            logger.LogWarning($"Unexpected name: {paramName} or spec: {typeSpecBuilder}");
                        }
                    }

                    // all parameter names and types must match 
                    foreach (var (pName, _) in parameterNamesAndTypes) {
                        var actionParm = actionMethod.GetParameters().SingleOrDefault(p => p.Name?.ToLower() == pName);
                        var choicesParm = methodToUse.GetParameters().SingleOrDefault(p => p.Name?.ToLower() == pName);

                        if (actionParm is null) {
                            logger.LogWarning($"Choices method: {methodToUse.DeclaringType}.{methodToUse.Name} has non matching parameter name: {pName}");
                            mismatchedParm = true;
                        }
                        else if (actionParm.ParameterType != choicesParm?.ParameterType) {
                            logger.LogWarning($"Choices method parameter: {methodToUse.DeclaringType}.{methodToUse.Name}.{pName} has non matching type: {choicesParm?.ParameterType} should be: {actionParm.ParameterType}");
                            mismatchedParm = true;
                        }
                    }

                    if (!mismatchedParm) {
                        FacetUtils.AddFacet(new ActionChoicesFacetViaFunction(methodToUse, parameterNamesAndTypes.ToArray(), returnType, parameters[i], LoggerFactory.CreateLogger<ActionChoicesFacetViaFunction>(), isMultiple));
                    }
                }
            }

            return metamodel;
        }

        private static bool Matches(MethodInfo methodInfo, string name, Type returnType) =>
            methodInfo.Name == name &&
            MatchReturnType(methodInfo.ReturnType, returnType);

        private static bool MatchReturnType(Type returnType, Type toMatch) => CollectionUtils.IsGenericEnumerable(returnType) && returnType.GenericTypeArguments.SequenceEqual(toMatch.GenericTypeArguments);

        #region IMethodFilteringFacetFactory Members

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo actionMethod, ISpecificationBuilder action, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var capitalizedName = NameUtils.CapitalizeName(actionMethod.Name);
            var declaringType = actionMethod.DeclaringType;
            var paramTypes = actionMethod.GetParameters().Select(p => p.ParameterType).ToArray();

            if (action is IActionSpecImmutable actionSpecImmutable) {
                var actionParameters = actionSpecImmutable.Parameters;
                metamodel = FindChoicesMethod(reflector, declaringType, capitalizedName, paramTypes, actionMethod, actionParameters, metamodel);
            }

            return metamodel;
        }

        public bool Filters(MethodInfo method, IClassStrategy classStrategy) => method.Name.StartsWith(RecognisedMethodsAndPrefixes.ParameterChoicesPrefix);

        #endregion
    }
}