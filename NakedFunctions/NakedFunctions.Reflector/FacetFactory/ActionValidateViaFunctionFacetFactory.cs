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
    public sealed class ActionValidateViaFunctionFacetFactory : FunctionalFacetFactoryProcessor, IMethodPrefixBasedFacetFactory, IMethodFilteringFacetFactory {
        private static readonly string[] FixedPrefixes = {
            RecognisedMethodsAndPrefixes.ValidatePrefix
        };

        private readonly ILogger<ActionValidateViaFunctionFacetFactory> logger;

        public ActionValidateViaFunctionFacetFactory(IFacetFactoryOrder<ActionValidateViaFunctionFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.Actions) =>
            logger = loggerFactory.CreateLogger<ActionValidateViaFunctionFacetFactory>();

        public string[] Prefixes => FixedPrefixes;

        #region IMethodFilteringFacetFactory Members

        private static bool Matches(MethodInfo methodInfo, string name, Type type, Type paramType) =>
            methodInfo.Name == name &&
            methodInfo.DeclaringType == type &&
            InjectUtils.FilterParms(methodInfo).Count(p => p.ParameterType == paramType) == 1 &&
            methodInfo.ReturnType == typeof(string);

        private static MethodInfo FindValidateMethod(Type declaringType, string capitalizedName, int i, Type paramType) {
            var name = RecognisedMethodsAndPrefixes.ValidatePrefix + i + capitalizedName;
            return declaringType.GetMethods().SingleOrDefault(methodInfo => Matches(methodInfo, name, declaringType, paramType));
        }

        private static IImmutableDictionary<string, ITypeSpecBuilder> FindValidateMethod(Type declaringType, string capitalizedName, Type[] paramTypes, IActionParameterSpecImmutable[] parameters, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            for (var i = 0; i < paramTypes.Length; i++) {
                var paramType = paramTypes[i];

                var methodToUse = FindValidateMethod(declaringType, capitalizedName, i, paramType);

                if (methodToUse is not null) {
                    // add facets directly to parameters, not to actions
                    FacetUtils.AddFacet(new ActionValidationViaFunctionFacet(methodToUse, parameters[i]));
                }
            }

            return metamodel;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo actionMethod, ISpecificationBuilder action, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var capitalizedName = NameUtils.CapitalizeName(actionMethod.Name);
            var declaringType = actionMethod.DeclaringType;

            var paramTypes = actionMethod.GetParameters().Select(p => p.ParameterType).ToArray();

            if (action is IActionSpecImmutable actionSpecImmutable) {
                var actionParameters = actionSpecImmutable.Parameters;
                metamodel = FindValidateMethod(declaringType, capitalizedName, paramTypes, actionParameters, metamodel);
            }

            return metamodel;
        }

        public bool Filters(MethodInfo method, IClassStrategy classStrategy) => method.Name.StartsWith(RecognisedMethodsAndPrefixes.ValidatePrefix);

        #endregion
    }
}