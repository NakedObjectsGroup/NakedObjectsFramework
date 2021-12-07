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
        { typeof(bool), (o, s) => new BooleanValueSemanticsProvider(o, s) }
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