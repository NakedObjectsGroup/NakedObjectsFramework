// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Reflect;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.SemanticsProvider;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Reflector.TypeFacetFactory {
    public abstract class ValueUsingValueSemanticsProviderFacetFactory : SystemTypeFacetFactoryProcessor {
        protected ValueUsingValueSemanticsProviderFacetFactory(int numericOrder, ILoggerFactory loggerFactory)
            : base(numericOrder, loggerFactory, FeatureType.ObjectsAndInterfaces) { }

        public static void AddValueFacets<T>(IValueSemanticsProvider<T> semanticsProvider, ISpecification holder) {
            FacetUtils.AddFacet(semanticsProvider as IFacet);

            // value implies aggregated
            FacetUtils.AddFacet(new AggregatedFacetAlways(holder));

            // ImmutableFacet, if appropriate
            var immutable = semanticsProvider == null || semanticsProvider.IsImmutable;
            if (immutable) {
                FacetUtils.AddFacet(new ImmutableFacetViaValueSemantics(holder));
            }

            // EqualByContentFacet, if appropriate
            var equalByContent = semanticsProvider == null || semanticsProvider.IsEqualByContent;
            if (equalByContent) {
                FacetUtils.AddFacet(new EqualByContentFacet(holder));
            }

            if (semanticsProvider != null) {
                FacetUtils.AddFacet(new EncodeableFacetUsingEncoderDecoder<T>(semanticsProvider, holder));
                FacetUtils.AddFacet(new ParseableFacetUsingParser<T>(semanticsProvider, holder));
                FacetUtils.AddFacet(new TitleFacetUsingParser<T>(semanticsProvider, holder));
                FacetUtils.AddFacet(new TypicalLengthFacetUsingParser<T>(semanticsProvider, holder));

                if (semanticsProvider is IFromStream fromStream) {
                    FacetUtils.AddFacet(new FromStreamFacetUsingFromStream(fromStream, holder));
                }

// ReSharper disable once CompareNonConstrainedGenericWithNull
                if (semanticsProvider.DefaultValue != null) {
                    FacetUtils.AddFacet(new DefaultedFacetUsingDefaultsProvider<T>(semanticsProvider, holder));
                }
            }
        }
    }
}