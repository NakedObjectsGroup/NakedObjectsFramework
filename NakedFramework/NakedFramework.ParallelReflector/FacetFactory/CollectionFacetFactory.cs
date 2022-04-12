// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;

namespace NakedFramework.ParallelReflector.FacetFactory;

public sealed class CollectionFacetFactory : SystemTypeFacetFactoryProcessor {
    public CollectionFacetFactory(IFacetFactoryOrder<CollectionFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.ObjectsInterfacesPropertiesAndCollections) { }

    private static IImmutableDictionary<string, ITypeSpecBuilder> ProcessArray(IReflector reflector, Type type, ISpecificationBuilder holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        FacetUtils.AddFacet(ArrayFacet.Instance, holder);
        FacetUtils.AddFacet(TypeOfFacetInferredFromArray.Instance, holder);

        var elementType = type.GetElementType();
        (_, metamodel) = reflector.LoadSpecification(elementType, metamodel);
        return metamodel;
    }

    private static void ProcessGenericEnumerable(Type type, ISpecificationBuilder holder) {
        var isCollection = CollectionUtils.IsGenericCollection(type); // as opposed to IEnumerable
        var isQueryable = CollectionUtils.IsGenericQueryable(type);
        var isSet = CollectionUtils.IsSet(type);

        FacetUtils.AddFacet(TypeOfFacetInferredFromGenerics.Instance, holder);

        var facet = GetCollectionFacet((isQueryable, isCollection, isSet));

        FacetUtils.AddFacet(facet, holder);
    }

    private static IFacet GetCollectionFacet((bool isQueryable, bool isCollection, bool isSet) conditions) =>
        conditions switch {
            (true, _, true) => GenericIQueryableSetFacet.Instance,
            (true, _, false) => GenericIQueryableFacet.Instance,
            (_, true, true) => GenericCollectionSetFacet.Instance,
            (_, true, false) => GenericCollectionFacet.Instance,
            (_, _, true) => GenericIEnumerableSetFacet.Instance,
            (_, _, false) => GenericIEnumerableFacet.Instance
        };

    private static IImmutableDictionary<string, ITypeSpecBuilder> ProcessCollection(IReflector reflector, ISpecificationBuilder holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        FacetUtils.AddFacet(TypeOfFacetDefaultToObject.Instance, holder);
        FacetUtils.AddFacet(CollectionFacet.Instance, holder);
        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        if (CollectionUtils.IsGenericEnumerable(type)) {
            ProcessGenericEnumerable(type, specification);
            return metamodel;
        }

        if (type.IsArray) {
            return ProcessArray(reflector, type, specification, metamodel);
        }

        return CollectionUtils.IsCollectionButNotArray(type)
            ? ProcessCollection(reflector, specification, metamodel)
            : metamodel;
    }
}