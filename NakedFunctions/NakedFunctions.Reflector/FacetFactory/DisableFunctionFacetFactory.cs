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
    public sealed class DisableFunctionFacetFactory : FunctionalFacetFactoryProcessor, IMethodPrefixBasedFacetFactory, IMethodFilteringFacetFactory {
        private static readonly string[] FixedPrefixes = {
            RecognisedMethodsAndPrefixes.DisablePrefix
        };

        private readonly ILogger<DisableFunctionFacetFactory> logger;

        public DisableFunctionFacetFactory(IFacetFactoryOrder<DisableFunctionFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.Actions) =>
            logger = loggerFactory.CreateLogger<DisableFunctionFacetFactory>();

        public string[] Prefixes => FixedPrefixes;

        #region IMethodFilteringFacetFactory Members

        private static bool Matches(MethodInfo methodInfo, string name, Type type) =>
            methodInfo.Name == name &&
            methodInfo.DeclaringType == type &&
            methodInfo.ReturnType == typeof(string) &&
            !InjectUtils.FilterParms(methodInfo).Any();

        private MethodInfo FindDisableMethod(Type declaringType, string name) {
            var disableMethod = declaringType.GetMethods().SingleOrDefault(methodInfo => Matches(methodInfo, name, declaringType));
            var nameMatches = declaringType.GetMethods().Where(mi => mi.Name == name && mi != disableMethod);

            foreach (var methodInfo in nameMatches) {
                logger.LogWarning($"disable method found: {methodInfo.Name} not matching expected signature");
            }

            return disableMethod;
        }

        private IImmutableDictionary<string, ITypeSpecBuilder> FindAndAddFacet(Type declaringType, string name, ISpecificationBuilder action, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var methodToUse = FindDisableMethod(declaringType, name);
            if (methodToUse is not null) {
                FacetUtils.AddFacet(new DisableForContextViaFunctionFacet(methodToUse, action));
            }

            return metamodel;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo actionMethod, ISpecificationBuilder action, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var name = $"{RecognisedMethodsAndPrefixes.DisablePrefix}{NameUtils.CapitalizeName(actionMethod.Name)}";
            var declaringType = actionMethod.DeclaringType;
            return FindAndAddFacet(declaringType, name, action, metamodel);
        }

        public bool Filters(MethodInfo method, IClassStrategy classStrategy) => method.Name.StartsWith(RecognisedMethodsAndPrefixes.DisablePrefix);

        #endregion
    }
}