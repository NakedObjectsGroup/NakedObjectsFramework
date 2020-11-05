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
using NakedObjects;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;
using NakedObjects.ParallelReflector.FacetFactory;

namespace NakedFunctions.Reflector.FacetFactory {
    public sealed class CollectionFieldMethodsFacetFactory : FunctionalFacetFactoryProcessor, IMethodPrefixBasedFacetFactory, IPropertyOrCollectionIdentifyingFacetFactory
    {
        private static readonly string[] FixedPrefixes = {
            RecognisedMethodsAndPrefixes.ModifyPrefix
        };

        public CollectionFieldMethodsFacetFactory(IFacetFactoryOrder<CollectionFieldMethodsFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.Collections) { }

        public string[] Prefixes => FixedPrefixes;

        private IList<PropertyInfo> PropertiesToBeIntrospected(IList<PropertyInfo> candidates, IClassStrategy classStrategy) =>
            candidates.Where(property => property.GetGetMethod() is not null &&
                                         classStrategy.IsTypeToBeIntrospected(property.PropertyType) &&
                                         !classStrategy.IsIgnored(property)).ToList();

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector,  PropertyInfo property,  ISpecificationBuilder collection, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var capitalizedName = property.Name;
            var type = property.DeclaringType;

            var facets = new List<IFacet> {new PropertyAccessorFacet(property, collection)};

            AddSetFacet(facets, property, collection);

            MethodHelpers.AddHideForSessionFacetNone(facets, collection);
            MethodHelpers.AddDisableFacetAlways(facets, collection);
            MethodHelpers.FindDefaultHideMethod(reflector, facets, property.DeclaringType, MethodType.Object, "PropertyDefault", collection, LoggerFactory);
            MethodHelpers.FindAndRemoveHideMethod(reflector, facets, type, MethodType.Object, capitalizedName, collection, LoggerFactory);
            FacetUtils.AddFacets(facets);
            return metamodel;
        }

        private static void AddSetFacet(ICollection<IFacet> collectionFacets, PropertyInfo property, ISpecification collection) {
            if (CollectionUtils.IsSet(property.PropertyType)) {
                collectionFacets.Add(new IsASetFacet(collection));
            }
        }

        public static IList<Type> BuildCollectionTypes(IEnumerable<PropertyInfo> properties) {
            return properties.Where(property => property.GetGetMethod() is not null &&
                                                CollectionUtils.IsCollection(property.PropertyType) &&
                                                !CollectionUtils.IsBlobOrClob(property.PropertyType) &&
                                                property.GetCustomAttribute<NakedObjectsIgnoreAttribute>() == null &&
                                                !CollectionUtils.IsQueryable(property.PropertyType)).Select(p => p.PropertyType).ToArray();
        }

        public override IList<PropertyInfo> FindCollectionProperties(IList<PropertyInfo> candidates, IClassStrategy classStrategy) {
            var collectionTypes = BuildCollectionTypes(candidates);
            candidates = candidates.Where(property => collectionTypes.Contains(property.PropertyType)).ToArray();
            return PropertiesToBeIntrospected(candidates, classStrategy);
        }
    }
}