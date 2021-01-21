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
using NakedFunctions.Reflector.Utils;
using NakedObjects;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;
using NakedObjects.ParallelReflector.FacetFactory;

namespace NakedFunctions.Reflector.FacetFactory {
    public sealed class ActionValidateViaFunctionFacetFactory : FunctionalFacetFactoryProcessor, IMethodPrefixBasedFacetFactory, IMethodFilteringFacetFactory {
        private static readonly string[] FixedPrefixes = {
            RecognisedMethodsAndPrefixes.ValidatePrefix
        };

        private readonly ILogger<ActionValidateViaFunctionFacetFactory> logger;

        public ActionValidateViaFunctionFacetFactory(IFacetFactoryOrder<ActionValidateViaFunctionFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.ActionsAndActionParameters) =>
            logger = loggerFactory.CreateLogger<ActionValidateViaFunctionFacetFactory>();

        public string[] Prefixes => FixedPrefixes;

        #region IMethodFilteringFacetFactory Members

        private static bool MatchParams(MethodInfo methodInfo, Type[] toMatch) {
            var actualParams = InjectUtils.FilterParms(methodInfo).Select(p => p.ParameterType).ToArray();
            if (actualParams.Length == 0 || actualParams.Length != toMatch.Length) {
                return false;
            }

            return actualParams.Zip(toMatch).All(i => i.First == i.Second);
        }

        private static bool Matches(MethodInfo methodInfo, string name, Type type, Type[] paramTypes) =>
            methodInfo.Matches(name, type, typeof(string)) &&
            MatchParams(methodInfo, paramTypes);

        private MethodInfo FindValidateMethod(Type declaringType, string name, Type[] paramTypes) {
            var validateMethods = declaringType.GetMethods().Where(methodInfo => Matches(methodInfo, name, declaringType, paramTypes)).ToArray();

            if (validateMethods.Length > 1) {
                logger.LogWarning($"Multiple methods found: {name} with matching signature - ignoring");
                return null;
            }

            var validateMethod = validateMethods.SingleOrDefault();
            var nameMatches = declaringType.GetMethods().Where(mi => mi.Name == name && mi != validateMethod);

            foreach (var methodInfo in nameMatches) {
                logger.LogWarning($"Method found: {methodInfo.Name} not matching expected signature");
            }

            return validateMethod;
        }

        private IImmutableDictionary<string, ITypeSpecBuilder> FindAndAddFacetToParameterValidateMethod(Type declaringType, string name, Type paramType, ISpecificationBuilder parameter, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var methodToUse = FindValidateMethod(declaringType, name, new[] {paramType});

            if (methodToUse is not null) {
                // add facets directly to parameters, not to actions
                FacetUtils.AddFacet(new ActionParameterValidationViaFunctionFacet(methodToUse, parameter));
            }

            return metamodel;
        }

        private IImmutableDictionary<string, ITypeSpecBuilder> FindAndAddFacetToActionValidateMethod(Type declaringType, string name, Type[] paramTypes, ISpecificationBuilder action, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var methodToUse = FindValidateMethod(declaringType, name, paramTypes);

            if (methodToUse is not null) {
                FacetUtils.AddFacet(new ActionValidationViaFunctionFacet(methodToUse, action));
            }

            return metamodel;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo actionMethod, ISpecificationBuilder action, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var name = $"{RecognisedMethodsAndPrefixes.ValidatePrefix}{NameUtils.CapitalizeName(actionMethod.Name)}";
            var declaringType = actionMethod.DeclaringType;
            var parameters = InjectUtils.FilterParms(actionMethod).Select(p => p.ParameterType).ToArray();

            return FindAndAddFacetToActionValidateMethod(declaringType, name, parameters, action, metamodel);
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> ProcessParams(IReflector reflector, MethodInfo actionMethod, int paramNum, ISpecificationBuilder holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var name = $"{RecognisedMethodsAndPrefixes.ValidatePrefix}{paramNum}{NameUtils.CapitalizeName(actionMethod.Name)}";
            var declaringType = actionMethod.DeclaringType;
            var parameter = actionMethod.GetParameters()[paramNum];

            return FindAndAddFacetToParameterValidateMethod(declaringType, name, parameter.ParameterType, holder, metamodel);
        }

        public bool Filters(MethodInfo method, IClassStrategy classStrategy) => method.Name.StartsWith(RecognisedMethodsAndPrefixes.ValidatePrefix);

        #endregion
    }
}