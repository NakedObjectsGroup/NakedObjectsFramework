// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Capabilities;
using NakedObjects.Meta.Spec;
using NakedObjects.Meta.Utils;
using NakedObjects.Util;

namespace NakedObjects.Meta.Facet {
    public abstract class ValueFacetAbstract<T> : MultipleValueFacetAbstract, IValueFacet {
        private readonly IValueSemanticsProvider<T> semanticsProvider;
        private readonly Specification specification = new Specification();

        protected ValueFacetAbstract(Type semanticsProviderClass, bool addFacetsIfInvalid, ISpecification holder)
            : this(NewValueSemanticsProviderOrNull(semanticsProviderClass), addFacetsIfInvalid, holder) {}

        protected ValueFacetAbstract(IValueSemanticsProvider<T> semanticsProvider, bool addFacetsIfInvalid, ISpecification holder)
            : base(Type, holder) {
            this.semanticsProvider = semanticsProvider;

            if (semanticsProvider == null && !addFacetsIfInvalid) {
                return;
            }

            // we now figure add all the facets supported.  Note that we do not use FacetUtil.addFacet,
            // because we need to add them explicitly to our delegate facetholder but have the
            // facets themselves reference this value's holder.

            specification.AddFacet((IFacet) this); // add just ValueFacet.class initially.

            // value implies aggregated 
            specification.AddFacet(new AggregatedFacetAlways(holder));

            // ImmutableFacet, if appropriate
            bool immutable = semanticsProvider == null || semanticsProvider.IsImmutable;
            if (immutable) {
                specification.AddFacet(new ImmutableFacetViaValueSemantics(holder));
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
                    specification.AddFacet(new EncodeableFacetUsingEncoderDecoder<T>(encoderDecoder, holder));
                }

                // install the ParseableFacet and other facets if we've been given a Parser
                IParser<T> parser = semanticsProvider.Parser;
                if (parser != null) {
                    specification.AddFacet(new ParseableFacetUsingParser<T>(parser, holder));
                    specification.AddFacet(new TitleFacetUsingParser<T>(parser, holder));
                    specification.AddFacet(new TypicalLengthFacetUsingParser<T>(parser, holder));
                }

                IFromStream fromStream = semanticsProvider.FromStream;
                if (fromStream != null) {
                    specification.AddFacet(new FromStreamFacetUsingFromStream(fromStream, holder));
                }

                // install the DefaultedFacet if we've been given a DefaultsProvider
                IDefaultsProvider<T> defaultsProvider = semanticsProvider.DefaultsProvider;
                if (defaultsProvider != null && defaultsProvider.DefaultValue != null) {
                    specification.AddFacet(new DefaultedFacetUsingDefaultsProvider<T>(defaultsProvider, holder));
                }
            }

            // ly, add self
            FacetUtils.AddFacet(this);
        }

        public static Type Type {
            get { return typeof (IValueFacet); }
        }

        #region IValueFacet Members

        public virtual bool IsValid {
            get { return semanticsProvider != null; }
        }

        public Type[] FacetTypes {
            get { return specification.FacetTypes; }
        }

        public IFacet GetFacet(Type facetType) {
            return specification.GetFacet(facetType);
        }

        public TF GetFacet<TF>() where TF : IFacet {
            return specification.GetFacet<TF>();
        }

        #endregion

        private static IValueSemanticsProvider<T> NewValueSemanticsProviderOrNull(Type semanticsProviderClass) {
            if (semanticsProviderClass == null) {
                return null;
            }
            return (IValueSemanticsProvider<T>) TypeUtils.NewInstance(semanticsProviderClass);
        }
    }
}