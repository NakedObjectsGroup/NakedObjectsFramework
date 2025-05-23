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
using NakedObjects.Reflector.Utils;

namespace NakedObjects.Reflector.FacetFactory;

public sealed class CollectionFieldMethodsFacetFactory : DomainObjectFacetFactoryProcessor, IMethodPrefixBasedFacetFactory, IPropertyOrCollectionIdentifyingFacetFactory {
    private static readonly string[] FixedPrefixes = {
        RecognisedMethodsAndPrefixes.ModifyPrefix
    };

    public CollectionFieldMethodsFacetFactory(IFacetFactoryOrder<CollectionFieldMethodsFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.Collections) { }

    public string[] Prefixes => FixedPrefixes;

    public override IList<PropertyInfo> FindCollectionProperties(IList<PropertyInfo> candidates, IClassStrategy classStrategy) {
        var collectionTypes = BuildCollectionTypes(candidates, classStrategy);
        candidates = candidates.Where(property => collectionTypes.Contains(property.PropertyType)).ToArray();
        return PropertiesToBeIntrospected(candidates, classStrategy);
    }

    private static IList<PropertyInfo> PropertiesToBeIntrospected(IList<PropertyInfo> candidates, IClassStrategy classStrategy) =>
        candidates.Where(property => property.HasPublicGetter() &&
                                     !classStrategy.IsIgnored(property.PropertyType) &&
                                     !classStrategy.IsIgnored(property)).ToList();

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder collection, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var capitalizedName = property.Name;
        var type = property.DeclaringType;

        var facets = new List<IFacet> { new PropertyAccessorFacet(property, Logger<PropertyAccessorFacet>()) };

        AddSetFacet(facets, property, collection);

        MethodHelpers.AddHideForSessionFacetNone(facets, collection);
        MethodHelpers.AddDisableFacetAlways(facets, collection);
        ObjectMethodHelpers.FindDefaultHideMethod(reflector, facets, property.DeclaringType, MethodType.Object, "PropertyDefault", collection, LoggerFactory);
        ObjectMethodHelpers.FindAndRemoveHideMethod(reflector, facets, type, MethodType.Object, capitalizedName, collection, LoggerFactory, methodRemover);
        FacetUtils.AddFacets(facets, collection);
        return metamodel;
    }

    private static void AddSetFacet(ICollection<IFacet> collectionFacets, PropertyInfo property, ISpecification collection) {
        if (CollectionUtils.IsSet(property.PropertyType)) {
            collectionFacets.Add(IsASetFacet.Instance);
        }
    }

    public static IList<Type> BuildCollectionTypes(IEnumerable<PropertyInfo> properties, IClassStrategy classStrategy) =>
        properties.Where(property => property.HasPublicGetter() &&
                                     CollectionUtils.IsCollection(property.PropertyType) &&
                                     !CollectionUtils.IsBlobOrClob(property.PropertyType) &&
                                     !classStrategy.IsIgnored(property) &&
                                     !CollectionUtils.IsQueryable(property.PropertyType)).Select(p => p.PropertyType).ToArray();
}