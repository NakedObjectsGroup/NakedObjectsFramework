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
using NakedFunctions.Meta.Facet;
using NakedFunctions.Reflector.Reflect;
using NakedObjects;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Utils;
using NakedObjects.ParallelReflector.FacetFactory;
using NakedObjects.Util;

namespace NakedFunctions.Reflector.FacetFactory {
    public sealed class ActionDefaultViaFunctionFacetFactory : FunctionalFacetFactoryProcessor, IMethodFilteringFacetFactory, IMethodPrefixBasedFacetFactory
    {
        private static readonly string[] FixedPrefixes = {
            RecognisedMethodsAndPrefixes.ParameterDefaultPrefix
        };

        private readonly ILogger<ActionDefaultViaFunctionFacetFactory> logger;

        public ActionDefaultViaFunctionFacetFactory(IFacetFactoryOrder<ActionDefaultViaFunctionFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.Actions) =>
            logger = loggerFactory.CreateLogger<ActionDefaultViaFunctionFacetFactory>();

        public  string[] Prefixes => FixedPrefixes;

        private static bool IsSameType(ParameterInfo pi, Type toMatch) =>
            pi != null &&
            pi.ParameterType == toMatch;

        private IImmutableDictionary<string, ITypeSpecBuilder> FindDefaultMethod(IReflector reflector, Type type, string capitalizedName, Type[] paramTypes, IActionParameterSpecImmutable[] parameters, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            for (var i = 0; i < paramTypes.Length; i++) {
                var paramType = paramTypes[i];

                var methodToUse = FindDefaultMethod(reflector, type, capitalizedName, i, paramType);

                if (methodToUse != null) {
                    // add facets directly to parameters, not to actions

                    FacetUtils.AddFacet(new ActionDefaultsFacetViaFunction(methodToUse, parameters[i]));
                }
            }

            return metamodel;
        }

        private static bool Matches(MethodInfo m, string name, Type type, Type returnType) =>
            m.Name == name &&
            m.DeclaringType == type &&
            m.ReturnType == returnType;

        private MethodInfo FindDefaultMethod(IReflector reflector, Type type, string capitalizedName, int i, Type returnType) {
            var name = RecognisedMethodsAndPrefixes.ParameterDefaultPrefix + i + capitalizedName;
            var match = FunctionalIntrospector.Functions.SelectMany(t => t.GetMethods())
                                              .Where(m => m.Name == name)
                                              .Where(m => m.ReturnType == returnType)
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
                metamodel = FindDefaultMethod(reflector, type, capitalizedName, paramTypes, actionParameters, metamodel);
            }

            return metamodel;
        }

        public bool Filters(MethodInfo method, IClassStrategy classStrategy) => method.Name.StartsWith(RecognisedMethodsAndPrefixes.ParameterDefaultPrefix);

        #endregion
    }
}