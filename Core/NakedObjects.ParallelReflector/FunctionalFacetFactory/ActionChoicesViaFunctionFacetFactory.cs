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
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using NakedFunctions;
using NakedFunctions.Meta.Facet;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Utils;
using NakedObjects.ParallelReflect.FacetFactory;
using NakedObjects.Util;

namespace NakedObjects.ParallelReflect.FunctionalFacetFactory {
    public sealed class ActionChoicesViaFunctionFacetFactory : MethodPrefixBasedFacetFactoryAbstract, IMethodFilteringFacetFactory {
        private static readonly string[] FixedPrefixes = {
            RecognisedMethodsAndPrefixes.ParameterChoicesPrefix
        };

        private readonly ILogger<ActionChoicesViaFunctionFacetFactory> logger;

        public ActionChoicesViaFunctionFacetFactory(IFacetFactoryOrder<ActionChoicesViaFunctionFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.Actions, ReflectionType.Functional) =>
            logger = loggerFactory.CreateLogger<ActionChoicesViaFunctionFacetFactory>();

        public override string[] Prefixes => FixedPrefixes;

        private static bool IsSameType(ParameterInfo pi, Type toMatch) =>
            pi != null &&
            pi.ParameterType == toMatch;

        private ParameterInfo[] FilterParms(MethodInfo m) =>
            m.GetParameters().Where(p => !p.IsDefined(typeof(InjectedAttribute), false) && (!m.IsDefined(typeof(ExtensionAttribute), false) || p.Position > 0))
             .ToArray();


        private IImmutableDictionary<string, ITypeSpecBuilder> FindChoicesMethod(IReflector reflector, Type type, string capitalizedName, Type[] paramTypes, IActionParameterSpecImmutable[] parameters, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            for (var i = 0; i < paramTypes.Length; i++) {
                var paramType = paramTypes[i];
                var isMultiple = false;

                if (CollectionUtils.IsGenericEnumerable(paramType)) {
                    paramType = paramType.GetGenericArguments().First();
                    isMultiple = true;
                }

                var returnType = typeof(IEnumerable<>).MakeGenericType(paramType);

                var methodToUse = FindChoicesMethod(reflector, type, capitalizedName, i, returnType);

                if (methodToUse != null) {
                    // add facets directly to parameters, not to actions
                    var parameterNamesAndTypes = new List<(string, IObjectSpecImmutable)>();

                    foreach (var p in FilterParms(methodToUse)) {
                        var result = reflector.LoadSpecification(p.ParameterType, metamodel);
                        metamodel = result.Item2;
                        var spec = result.Item1 as IObjectSpecImmutable;
                        var name = p.Name.ToLower();
                        parameterNamesAndTypes.Add((name, spec));
                    }

                    FacetUtils.AddFacet(new ActionChoicesFacetViaFunction(methodToUse, parameterNamesAndTypes.ToArray(), returnType, parameters[i], isMultiple));
                    AddOrAddToExecutedWhereFacet(methodToUse, parameters[i]);
                }
            }

            return metamodel;
        }

        private static bool Matches(MethodInfo m, string name, Type type, Type returnType) =>
            m.Name == name &&
            m.DeclaringType == type &&
            MatchReturnType(m.ReturnType, returnType);

        private static bool MatchReturnType(Type returnType, Type toMatch) => CollectionUtils.IsGenericEnumerable(returnType) && returnType.GenericTypeArguments.SequenceEqual(toMatch.GenericTypeArguments);


        private MethodInfo FindChoicesMethod(IReflector reflector, Type type, string capitalizedName, int i, Type returnType) {
            var name = RecognisedMethodsAndPrefixes.ParameterChoicesPrefix + i + capitalizedName;
            var match = FunctionalIntrospector.Functions.SelectMany(t => t.GetMethods())
                                              .Where(m => m.Name == name)
                                              .SingleOrDefault(m => Matches(m, name, type, returnType));

            return match;
        }

        #region IMethodFilteringFacetFactory Members

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo actionMethod, IMethodRemover methodRemover, ISpecificationBuilder action, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var capitalizedName = NameUtils.CapitalizeName(actionMethod.Name);

            var type = actionMethod.DeclaringType;

            var paramTypes = actionMethod.GetParameters().Select(p => p.ParameterType).ToArray();

            var actionSpecImmutable = action as IActionSpecImmutable;
            if (actionSpecImmutable != null) {
                var actionParameters = actionSpecImmutable.Parameters;
                metamodel = FindChoicesMethod(reflector, type, capitalizedName, paramTypes, actionParameters, metamodel);
            }

            return metamodel;
        }

        public bool Filters(MethodInfo method, IClassStrategy classStrategy) => method.Name.StartsWith(RecognisedMethodsAndPrefixes.ParameterChoicesPrefix);

        #endregion
    }
}