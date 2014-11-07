// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Capabilities;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Reflect.TypeFacetFactory {
    public abstract class ValueUsingValueSemanticsProviderFacetFactory : FacetFactoryAbstract {
        protected ValueUsingValueSemanticsProviderFacetFactory(IReflector reflector)
            : base(reflector, FeatureType.Objects) {}

        public static bool AddValueFacets<T>(IValueSemanticsProvider<T> semanticsProvider, ISpecification holder) {
            FacetUtils.AddFacet(semanticsProvider as IFacet);

            // we now figure add all the facets supported.  Note that we do not use FacetUtil.addFacet,
            // because we need to add them explicitly to our delegate facetholder but have the
            // facets themselves reference this value's holder.


            // value implies aggregated 
            FacetUtils.AddFacet(new AggregatedFacetAlways(holder));

            // ImmutableFacet, if appropriate
            bool immutable = semanticsProvider == null || semanticsProvider.IsImmutable;
            if (immutable) {
                FacetUtils.AddFacet(new ImmutableFacetViaValueSemantics(holder));
            }

            // EqualByContentFacet, if appropriate
            bool equalByContent = semanticsProvider == null || semanticsProvider.IsEqualByContent;
            if (equalByContent) {
                FacetUtils.AddFacet(new EqualByContentFacet(holder));
            }

            if (semanticsProvider != null) {
                // install the EncodeableFacet if we've been given an EncoderDecoder
                IEncoderDecoder<T> encoderDecoder = semanticsProvider.EncoderDecoder;
                if (encoderDecoder != null) {
                    FacetUtils.AddFacet(new EncodeableFacetUsingEncoderDecoder<T>(encoderDecoder, holder));
                }

                // install the ParseableFacet and other facets if we've been given a Parser
                IParser<T> parser = semanticsProvider.Parser;
                if (parser != null) {
                    FacetUtils.AddFacet(new ParseableFacetUsingParser<T>(parser, holder));
                    FacetUtils.AddFacet(new TitleFacetUsingParser<T>(parser, holder));
                    FacetUtils.AddFacet(new TypicalLengthFacetUsingParser<T>(parser, holder));
                }

                IFromStream fromStream = semanticsProvider.FromStream;
                if (fromStream != null) {
                    FacetUtils.AddFacet(new FromStreamFacetUsingFromStream(fromStream, holder));
                }

                // install the DefaultedFacet if we've been given a DefaultsProvider
                IDefaultsProvider<T> defaultsProvider = semanticsProvider.DefaultsProvider;
                if (defaultsProvider != null && defaultsProvider.DefaultValue != null) {
                    FacetUtils.AddFacet(new DefaultedFacetUsingDefaultsProvider<T>(defaultsProvider, holder));
                }
            }

            return true;
        }
    }
}