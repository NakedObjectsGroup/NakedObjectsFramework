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
using NakedFramework;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Utils;
using NakedFramework.ParallelReflector.FacetFactory;
using NakedFunctions.Reflector.Facet;
using NakedFunctions.Reflector.Utils;

namespace NakedFunctions.Reflector.FacetFactory;

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

    private static bool Matches(MethodInfo methodInfo, string name, Type declaringType, Type targetType) =>
        methodInfo.Matches(name, declaringType, typeof(string), targetType) &&
        !InjectUtils.FilterParms(methodInfo).Any();

    private IImmutableDictionary<string, ITypeSpecBuilder> FindAndAddFacet(Type declaringType, Type targetType, string name, ISpecificationBuilder action, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        bool Matcher(MethodInfo mi) => Matches(mi, name, declaringType, targetType);
        var methodToUse = FactoryUtils.FindComplementaryMethod(declaringType, name, Matcher, logger);
        if (methodToUse is not null) {
            FacetUtils.AddFacet(new DisableForContextViaFunctionFacet(methodToUse, action, Logger<DisableForContextViaFunctionFacet>()), action);
        }

        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo actionMethod, ISpecificationBuilder action, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var name = $"{RecognisedMethodsAndPrefixes.DisablePrefix}{NameUtils.CapitalizeName(actionMethod.Name)}";
        var declaringType = actionMethod.DeclaringType;
        var targetType = actionMethod.ContributedToType();
        return FindAndAddFacet(declaringType, targetType, name, action, metamodel);
    }

    public bool Filters(MethodInfo method, IClassStrategy classStrategy) => method.Name.StartsWith(RecognisedMethodsAndPrefixes.DisablePrefix);

    #endregion
}