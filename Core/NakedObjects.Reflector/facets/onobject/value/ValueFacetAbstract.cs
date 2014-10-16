// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Value;
using NakedObjects.Capabilities;
using NakedObjects.Reflector.DotNet.Facets.Objects.Defaults;
using NakedObjects.Reflector.DotNet.Facets.Objects.Encodeable;
using NakedObjects.Reflector.DotNet.Facets.Objects.FromStream;
using NakedObjects.Reflector.DotNet.Facets.Objects.Ident.Title;
using NakedObjects.Reflector.DotNet.Facets.Objects.Parseable;
using NakedObjects.Reflector.DotNet.Facets.Propparam.TypicalLength;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Value {
    public abstract class ValueFacetAbstract<T> : MultipleValueFacetAbstract, IValueFacet {
        private readonly FacetHolderImpl facetHolder = new FacetHolderImpl();

        private readonly IValueSemanticsProvider<T> semanticsProvider;

        protected ValueFacetAbstract(Type semanticsProviderClass, bool addFacetsIfInvalid, IFacetHolder holder)
            : this(NewValueSemanticsProviderOrNull(semanticsProviderClass), addFacetsIfInvalid, holder) {}

        protected ValueFacetAbstract(IValueSemanticsProvider<T> semanticsProvider, bool addFacetsIfInvalid, IFacetHolder holder)
            : base(Type, holder) {
            this.semanticsProvider = semanticsProvider;

            if (semanticsProvider == null && !addFacetsIfInvalid) {
                return;
            }

            // we now figure add all the facets supported.  Note that we do not use FacetUtil.addFacet,
            // because we need to add them explicitly to our delegate facetholder but have the
            // facets themselves reference this value's holder.

            facetHolder.AddFacet((IFacet) this); // add just ValueFacet.class initially.

            // value implies aggregated 
            facetHolder.AddFacet(new AggregatedFacetAlways(holder));

            // ImmutableFacet, if appropriate
            bool immutable = semanticsProvider == null || semanticsProvider.IsImmutable;
            if (immutable) {
                facetHolder.AddFacet(new ImmutableFacetViaValueSemantics(holder));
            }

            // EqualByContentFacet, if appropriate
            bool equalByContent = semanticsProvider == null || semanticsProvider.IsEqualByContent;
            if (equalByContent) {
                FacetUtils.AddFacet(new EqualByContentFacetViaValueSemantics(holder));
            }

            if (semanticsProvider != null) {
                // install the EncodeableFacet if we've been given an EncoderDecoder
                IEncoderDecoder<T> encoderDecoder = semanticsProvider.EncoderDecoder;
                if (encoderDecoder != null) {
                    facetHolder.AddFacet(new EncodeableFacetUsingEncoderDecoder<T>(encoderDecoder, holder));
                }

                // install the ParseableFacet and other facets if we've been given a Parser
                IParser<T> parser = semanticsProvider.Parser;
                if (parser != null) {
                    facetHolder.AddFacet(new ParseableFacetUsingParser<T>(parser, holder));
                    facetHolder.AddFacet(new TitleFacetUsingParser<T>(parser, holder));
                    facetHolder.AddFacet(new TypicalLengthFacetUsingParser<T>(parser, holder));
                }

                IFromStream fromStream = semanticsProvider.FromStream;
                if (fromStream != null) {
                    facetHolder.AddFacet(new FromStreamFacetUsingFromStream(fromStream, holder));
                }

                // install the DefaultedFacet if we've been given a DefaultsProvider
                IDefaultsProvider<T> defaultsProvider = semanticsProvider.DefaultsProvider;
                if (defaultsProvider != null && defaultsProvider.DefaultValue != null) {
                    facetHolder.AddFacet(new DefaultedFacetUsingDefaultsProvider<T>(defaultsProvider, holder));
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
            get { return facetHolder.FacetTypes; }
        }

        public IFacet GetFacet(Type facetType) {
            return facetHolder.GetFacet(facetType);
        }

        public TF GetFacet<TF>() where TF : IFacet {
            return facetHolder.GetFacet<TF>();
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