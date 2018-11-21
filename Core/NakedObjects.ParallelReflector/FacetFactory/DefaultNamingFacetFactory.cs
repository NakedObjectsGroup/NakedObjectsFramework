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
        private static readonly ILog Log = LogManager.GetLogger(typeof (DefaultNamingFacetFactory));

        public DefaultNamingFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.Objects) {}

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var facets = new List<IFacet>();
            var namedFacet = specification.GetFacet<INamedFacet>();
            if (namedFacet == null) {
                namedFacet = new NamedFacetInferred(type.Name, specification);
                facets.Add(namedFacet);
                Log.InfoFormat("No name facet found inferring name {0}", type.Name);
            }

            var pluralFacet = specification.GetFacet<IPluralFacet>();
            if (pluralFacet == null) {
                string pluralName = NameUtils.PluralName(namedFacet.NaturalName);
                pluralFacet = new PluralFacetInferred(pluralName, specification);
                facets.Add(pluralFacet);
                Log.InfoFormat("No plural facet found inferring name {0}", pluralName);
            }

            FacetUtils.AddFacets(facets);
            return metamodel;
        }
    }
}