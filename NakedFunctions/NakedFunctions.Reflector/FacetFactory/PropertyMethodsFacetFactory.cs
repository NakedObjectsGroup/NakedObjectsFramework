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
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;
using NakedFramework.ParallelReflector.FacetFactory;
using NakedFramework.ParallelReflector.Utils;

namespace NakedFunctions.Reflector.FacetFactory;

public sealed class PropertyMethodsFacetFactory : FunctionalFacetFactoryProcessor, IMethodPrefixBasedFacetFactory, IPropertyOrCollectionIdentifyingFacetFactory {
    private static readonly string[] FixedPrefixes = {
        RecognisedMethodsAndPrefixes.ModifyPrefix
    };

    private readonly ILogger<PropertyMethodsFacetFactory> logger;

    public PropertyMethodsFacetFactory(IFacetFactoryOrder<PropertyMethodsFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.Properties) =>
        logger = loggerFactory.CreateLogger<PropertyMethodsFacetFactory>();

    public string[] Prefixes => FixedPrefixes;

    public override IList<PropertyInfo> FindProperties(IList<PropertyInfo> candidates, IClassStrategy classStrategy) {
        candidates = candidates.Where(property => !CollectionUtils.IsQueryable(property.PropertyType)).ToArray();
        return PropertiesToBeIntrospected(candidates, classStrategy);
    }

    private static IList<PropertyInfo> PropertiesToBeIntrospected(IList<PropertyInfo> candidates, IClassStrategy classStrategy) =>
        candidates.Where(property => property.HasPublicGetter() &&
                                     !classStrategy.IsIgnored(property.PropertyType) &&
                                     !classStrategy.IsIgnored(property)).ToList();

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var facets = new List<IFacet> { new PropertyAccessorFacet(property, Logger<PropertyAccessorFacet>()) };

        if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
            facets.Add(NullableFacetAlways.Instance);
        }

        if (property.GetSetMethod() is not null) {
            facets.Add(DisabledFacetAlways.Instance);
            facets.Add(new PropertyInitializationFacet(property, Logger<PropertyInitializationFacet>()));
        }
        else {
            facets.Add(NotPersistedFacet.Instance);
            facets.Add(DisabledFacetAlways.Instance);
        }

        MethodHelpers.AddHideForSessionFacetNone(facets, specification);
        MethodHelpers.AddDisableForSessionFacetNone(facets, specification);

        FacetUtils.AddFacets(facets, specification);
        return metamodel;
    }
}