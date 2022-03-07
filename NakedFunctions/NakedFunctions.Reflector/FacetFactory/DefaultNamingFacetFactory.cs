// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using NakedFramework;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;

namespace NakedFunctions.Reflector.FacetFactory;

public sealed class DefaultNamingFacetFactory : FunctionalFacetFactoryProcessor, IAnnotationBasedFacetFactory {
    private readonly ILogger<DefaultNamingFacetFactory> logger;

    public DefaultNamingFacetFactory(IFacetFactoryOrder<DefaultNamingFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.ObjectsAndInterfaces) =>
        logger = loggerFactory.CreateLogger<DefaultNamingFacetFactory>();

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var facets = new List<IFacet>();
        var namedFacet = specification.GetFacet<INamedFacet>();
        if (namedFacet is null) {
            namedFacet = new NamedFacetInferred(type.Name, specification);
            facets.Add(namedFacet);
        }

        var pluralFacet = specification.GetFacet<IPluralFacet>();
        if (pluralFacet is null) {
            var pluralName = NameUtils.PluralName(namedFacet.FriendlyName);
            pluralFacet = new PluralFacetInferred(pluralName, specification);
            facets.Add(pluralFacet);
        }

        FacetUtils.AddFacets(facets, specification);
        return metamodel;
    }
}