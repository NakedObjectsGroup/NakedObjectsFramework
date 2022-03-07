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
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Utils;
using NakedFramework.ParallelReflector.FacetFactory;
using NakedFunctions.Reflector.Facet;
using NakedFunctions.Reflector.Utils;

namespace NakedFunctions.Reflector.FacetFactory;

public sealed class HideFunctionIntegrationFacetFactory : FunctionalFacetFactoryProcessor, IMethodPrefixBasedFacetFactory, IMethodFilteringFacetFactory {
    private static readonly string[] FixedPrefixes = {
        RecognisedMethodsAndPrefixes.HidePrefix
    };

    private readonly ILogger<HideFunctionIntegrationFacetFactory> logger;

    public HideFunctionIntegrationFacetFactory(IFacetFactoryOrder<HideFunctionIntegrationFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.Objects | FeatureType.Actions) =>
        logger = loggerFactory.CreateLogger<HideFunctionIntegrationFacetFactory>();

    public string[] Prefixes => FixedPrefixes;

    #region IMethodFilteringFacetFactory Members

    private static bool Matches(MethodInfo methodInfo, string name, Type declaringType, Type targetType) =>
        methodInfo.Matches(name, declaringType, typeof(bool), targetType) &&
        !InjectUtils.FilterParms(methodInfo).Any();

    private MethodInfo MethodToUse(Type declaringType, Type targetType, string name) {
        bool Matcher(MethodInfo mi) => Matches(mi, name, declaringType, targetType);
        var methodToUse = FactoryUtils.FindComplementaryMethod(declaringType, name, Matcher, logger);
        return methodToUse;
    }

    private IImmutableDictionary<string, ITypeSpecBuilder> FindAndAddFacet(Type declaringType, Type targetType, string name, ISpecificationBuilder action, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var methodToUse = MethodToUse(declaringType, targetType, name);
        if (methodToUse is not null) {
            FacetUtils.AddFacet(new HideForContextViaFunctionFacet(methodToUse, action, Logger<HideForContextViaFunctionFacet>()), action);
        }

        return metamodel;
    }

    private static string HiddenName(MethodInfo action) => $"{RecognisedMethodsAndPrefixes.HidePrefix}{NameUtils.CapitalizeName(action.Name)}";

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo actionMethod, ISpecificationBuilder action, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var name = HiddenName(actionMethod);
        var declaringType = actionMethod.DeclaringType;
        var targetType = actionMethod.ContributedToType();
        return FindAndAddFacet(declaringType, targetType, name, action, metamodel);
    }

    private Action<IMetamodelBuilder> GetAddAction(MethodInfo recognizedMethod) {
        Action<IMetamodelBuilder> action = m => { };

        var onType = recognizedMethod.ContributedToType();
        if (onType is not null) {
            action = m => {
                if (m.GetSpecification(onType) is ITypeSpecBuilder spec) {
                    var propertyName = recognizedMethod.Name.Remove(0, 4);
                    var property = spec.UnorderedFields.SingleOrDefault(f => f.Identifier.MemberName == propertyName);

                    if (property is not null) {
                        var facet = new HideForContextViaFunctionFacet(recognizedMethod, property, Logger<HideForContextViaFunctionFacet>());
                        FacetUtils.AddFacet(facet, property);
                    }
                    else {
                        logger.LogWarning($"No matching property found for {recognizedMethod.Name} on {spec.FullName}");
                    }
                }
            };
        }

        return action;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var matchedHides = type.GetMethods().Where(m => !m.Name.StartsWith("Hide")).Where(m => MethodToUse(type, m.ContributedToType(), HiddenName(m)) is not null);
        var unMatchedHides = type.GetMethods().Where(m => m.Name.StartsWith("Hide")).Except(matchedHides).ToArray();

        if (unMatchedHides.Any()) {
            var actions = unMatchedHides.Select(GetAddAction);
            void Action(IMetamodelBuilder m) => actions.ForEach(a => a(m));
            metamodel = FacetUtils.AddIntegrationFacet(reflector, type, Action, metamodel);
        }

        return metamodel;
    }

    public bool Filters(MethodInfo method, IClassStrategy classStrategy) => method.Name.StartsWith(RecognisedMethodsAndPrefixes.HidePrefix);

    #endregion
}