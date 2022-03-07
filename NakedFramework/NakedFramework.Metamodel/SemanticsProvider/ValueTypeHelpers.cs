using System;
using System.Collections.Generic;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;

namespace NakedFramework.Metamodel.SemanticsProvider;

public static class ValueTypeHelpers {
    private static readonly List<KeyValuePair<Type, Func<IObjectSpecImmutable, ISpecification, IValueSemanticsProvider>>> Factories = new() {
        BooleanValueSemanticsProvider.Factory,
        ByteValueSemanticsProvider.Factory,
        CharValueSemanticsProvider.Factory,
        ColorValueSemanticsProvider.Factory,
        DateTimeValueSemanticsProvider.Factory,
        DecimalValueSemanticsProvider.Factory,
        DoubleValueSemanticsProvider.Factory,
        FileAttachmentValueSemanticsProvider.Factory,
        FloatValueSemanticsProvider.Factory,
        GuidValueSemanticsProvider.Factory,
        ImageValueSemanticsProvider.Factory,
        IntValueSemanticsProvider.Factory,
        LongValueSemanticsProvider.Factory,
        SbyteValueSemanticsProvider.Factory,
        ShortValueSemanticsProvider.Factory,
        StringValueSemanticsProvider.Factory,
        TimeValueSemanticsProvider.Factory,
        UIntValueSemanticsProvider.Factory,
        ULongValueSemanticsProvider.Factory,
        UShortValueSemanticsProvider.Factory
    };

    public static readonly Dictionary<Type, Func<IObjectSpecImmutable, ISpecification, IValueSemanticsProvider>> TypeToSemanticProvider = new(Factories);

    public static void AddValueFacets<T>(IValueSemanticsProvider<T> semanticsProvider, ISpecificationBuilder holder) {
        var facets = new List<IFacet> {
            semanticsProvider as IFacet,
            // value implies aggregated
            new AggregatedFacetAlways(holder),
            new ParseableFacetUsingParser<T>(semanticsProvider, holder),
            new TitleFacetUsingParser<T>(semanticsProvider, holder),
            new ValueFacetFromSemanticProvider<T>(semanticsProvider, holder)
        };

        // ImmutableFacet, if appropriate
        if (semanticsProvider.IsImmutable) {
            facets.Add(new ImmutableFacetViaValueSemantics(holder));
        }

        if (semanticsProvider is IFromStream fromStream) {
            facets.Add(new FromStreamFacetUsingFromStream(fromStream, holder));
        }

        // ReSharper disable once CompareNonConstrainedGenericWithNull
        if (semanticsProvider.DefaultValue is not null) {
            facets.Add(new DefaultedFacetUsingDefaultsProvider<T>(semanticsProvider, holder));
        }

        FacetUtils.AddFacets(facets, holder);
    }
}