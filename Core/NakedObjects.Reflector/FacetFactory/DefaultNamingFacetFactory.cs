// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Common.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;
using NakedObjects.Util;

namespace NakedObjects.Reflect.FacetFactory {
    public sealed class DefaultNamingFacetFactory : AnnotationBasedFacetFactoryAbstract {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DefaultNamingFacetFactory));

        public DefaultNamingFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.ObjectsAndInterfaces) { }

        public override void Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            var namedFacet = specification.GetFacet<INamedFacet>() ?? new NamedFacetInferred(type.Name, specification);
            IFacet pluralFacet = specification.GetFacet<IPluralFacet>() ?? new PluralFacetInferred(NameUtils.PluralName(namedFacet.NaturalName), specification);
            var facets = new[] {namedFacet, pluralFacet};

            FacetUtils.AddFacets(facets);
        }
    }
}