// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Common.Logging;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;
using NakedObjects.Util;

namespace NakedObjects.ParallelReflect.FacetFactory {
    public sealed class DefaultNamingFacetFactory : AnnotationBasedFacetFactoryAbstract {
        private ILogger<DefaultNamingFacetFactory> logger;

        public DefaultNamingFacetFactory(int numericOrder, ILoggerFactory loggerFactory)
            : base(numericOrder, loggerFactory, FeatureType.ObjectsAndInterfaces) =>
            logger = loggerFactory.CreateLogger<DefaultNamingFacetFactory>();

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var facets = new List<IFacet>();
            var namedFacet = specification.GetFacet<INamedFacet>();
            if (namedFacet == null) {
                namedFacet = new NamedFacetInferred(type.Name, specification);
                facets.Add(namedFacet);
            }

            var pluralFacet = specification.GetFacet<IPluralFacet>();
            if (pluralFacet == null) {
                var pluralName = NameUtils.PluralName(namedFacet.NaturalName);
                pluralFacet = new PluralFacetInferred(pluralName, specification);
                facets.Add(pluralFacet);
            }

            FacetUtils.AddFacets(facets);
            return metamodel;
        }
    }
}