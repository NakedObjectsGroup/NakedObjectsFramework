using System;
using System.Collections.Generic;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;

namespace NakedFramework.Metamodel.SemanticsProvider;

public static class ValueTypeHelpers {
    public static readonly Dictionary<Type, Func<IObjectSpecImmutable, ISpecification, IValueSemanticsProvider>> TypeToSemanticProvider = new() {
        { BooleanValueSemanticsProvider.AdaptedType, (o, s) => new BooleanValueSemanticsProvider(o, s) },
        { ByteValueSemanticsProvider.AdaptedType, (o, s) => new ByteValueSemanticsProvider(o, s) },
        { CharValueSemanticsProvider.AdaptedType, (o, s) => new CharValueSemanticsProvider(o, s) },
        { ColorValueSemanticsProvider.AdaptedType, (o, s) => new ColorValueSemanticsProvider(o, s) },
        { DateTimeValueSemanticsProvider.AdaptedType, (o, s) => new DateTimeValueSemanticsProvider(o, s) },
        { DecimalValueSemanticsProvider.AdaptedType, (o, s) => new DecimalValueSemanticsProvider(o, s) },
        { DoubleValueSemanticsProvider.AdaptedType, (o, s) => new DoubleValueSemanticsProvider(o, s) },
        { FileAttachmentValueSemanticsProvider.AdaptedType, (o, s) => new FileAttachmentValueSemanticsProvider(o, s) },
        { FloatValueSemanticsProvider.AdaptedType, (o, s) => new FloatValueSemanticsProvider(o, s) },
        { GuidValueSemanticsProvider.AdaptedType, (o, s) => new GuidValueSemanticsProvider(o, s) },
        { ImageValueSemanticsProvider.AdaptedType, (o, s) => new ImageValueSemanticsProvider(o, s) },
        { IntValueSemanticsProvider.AdaptedType, (o, s) => new IntValueSemanticsProvider(o, s) },
        { LongValueSemanticsProvider.AdaptedType, (o, s) => new LongValueSemanticsProvider(o, s) },
        { SbyteValueSemanticsProvider.AdaptedType, (o, s) => new SbyteValueSemanticsProvider(o, s) },
        { ShortValueSemanticsProvider.AdaptedType, (o, s) => new ShortValueSemanticsProvider(o, s) },
        { StringValueSemanticsProvider.AdaptedType, (o, s) => new StringValueSemanticsProvider(o, s) },
        { TimeValueSemanticsProvider.AdaptedType, (o, s) => new TimeValueSemanticsProvider(o, s) },
        { UIntValueSemanticsProvider.AdaptedType, (o, s) => new UIntValueSemanticsProvider(o, s) },
        { ULongValueSemanticsProvider.AdaptedType, (o, s) => new ULongValueSemanticsProvider(o, s) },
        { UShortValueSemanticsProvider.AdaptedType, (o, s) => new UShortValueSemanticsProvider(o, s) }
    };

    public static void AddValueFacets<T>(IValueSemanticsProvider<T> semanticsProvider, ISpecification holder) {
        FacetUtils.AddFacet(semanticsProvider as IFacet);

        // value implies aggregated
        FacetUtils.AddFacet(new AggregatedFacetAlways(holder));

        // ImmutableFacet, if appropriate
        if (semanticsProvider.IsImmutable) {
            FacetUtils.AddFacet(new ImmutableFacetViaValueSemantics(holder));
        }

        FacetUtils.AddFacet(new ParseableFacetUsingParser<T>(semanticsProvider, holder));
        FacetUtils.AddFacet(new TitleFacetUsingParser<T>(semanticsProvider, holder));

        if (semanticsProvider is IFromStream fromStream) {
            FacetUtils.AddFacet(new FromStreamFacetUsingFromStream(fromStream, holder));
        }

        // ReSharper disable once CompareNonConstrainedGenericWithNull
        if (semanticsProvider.DefaultValue is not null) {
            FacetUtils.AddFacet(new DefaultedFacetUsingDefaultsProvider<T>(semanticsProvider, holder));
        }
    }
}