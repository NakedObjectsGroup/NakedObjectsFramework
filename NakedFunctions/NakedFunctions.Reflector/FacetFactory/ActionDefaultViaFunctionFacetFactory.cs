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
using NakedObjects;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Utils;
using NakedObjects.ParallelReflector.FacetFactory;

namespace NakedFunctions.Reflector.FacetFactory {
    public sealed class ActionDefaultViaFunctionFacetFactory : FunctionalFacetFactoryProcessor, IMethodFilteringFacetFactory, IMethodPrefixBasedFacetFactory {
        private static readonly string[] FixedPrefixes = {
            RecognisedMethodsAndPrefixes.ParameterDefaultPrefix
        };

        private readonly ILogger<ActionDefaultViaFunctionFacetFactory> logger;

        public ActionDefaultViaFunctionFacetFactory(IFacetFactoryOrder<ActionDefaultViaFunctionFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.Actions) =>
            logger = loggerFactory.CreateLogger<ActionDefaultViaFunctionFacetFactory>();

        public string[] Prefixes => FixedPrefixes;

        private IImmutableDictionary<string, ITypeSpecBuilder> FindDefaultMethod(Type declaringType, string capitalizedName, Type[] paramTypes, IActionParameterSpecImmutable[] parameters, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            for (var i = 0; i < paramTypes.Length; i++) {
                var paramType = paramTypes[i];

                var methodToUse = FindDefaultMethod(declaringType, capitalizedName, i, paramType);

                if (methodToUse is not null) {
                    // add facets directly to parameters, not to actions
                    FacetUtils.AddFacet(new ActionDefaultsFacetViaFunction(methodToUse, parameters[i]));
                }
            }

            return metamodel;
        }

        private static bool Matches(MethodInfo methodInfo, string name, Type type, Type returnType) =>
            methodInfo.Name == name &&
            methodInfo.DeclaringType == type &&
            methodInfo.ReturnType == returnType;

        private MethodInfo FindDefaultMethod(Type declaringType, string capitalizedName, int i, Type returnType) {
            var name = $"{RecognisedMethodsAndPrefixes.ParameterDefaultPrefix}{i}{capitalizedName}";
            var defaultMethod = declaringType.GetMethods().SingleOrDefault(methodInfo => Matches(methodInfo, name, declaringType, returnType));
            var nameMatches = declaringType.GetMethods().Where(mi => mi.Name == name && mi != defaultMethod);

            foreach (var methodInfo in nameMatches) {
                logger.LogWarning($"default method found: {methodInfo.Name} not matching expected signature");
            }

            return defaultMethod;
        }

        #region IMethodFilteringFacetFactory Members

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo actionMethod,  ISpecificationBuilder action, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var capitalizedName = NameUtils.CapitalizeName(actionMethod.Name);

            var declaringType = actionMethod.DeclaringType;

            var paramTypes = actionMethod.GetParameters().Select(p => p.ParameterType).ToArray();

            if (action is IActionSpecImmutable actionSpecImmutable) {
                var actionParameters = actionSpecImmutable.Parameters;
                metamodel = FindDefaultMethod(declaringType, capitalizedName, paramTypes, actionParameters, metamodel);
            }

            return metamodel;
        }

        public bool Filters(MethodInfo method, IClassStrategy classStrategy) => method.Name.StartsWith(RecognisedMethodsAndPrefixes.ParameterDefaultPrefix);

        #endregion
    }
}