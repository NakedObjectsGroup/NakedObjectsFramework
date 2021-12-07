// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.SemanticsProvider;
using NakedFramework.Metamodel.Utils;

namespace NakedFramework.ParallelReflector.TypeFacetFactory;

public abstract class ValueUsingValueSemanticsProviderFacetFactory : SystemTypeFacetFactoryProcessor {
    protected ValueUsingValueSemanticsProviderFacetFactory(int numericOrder, ILoggerFactory loggerFactory)
        : base(numericOrder, loggerFactory, FeatureType.ObjectsAndInterfaces) { }

    protected static void AddValueFacets<T>(IValueSemanticsProvider<T> semanticsProvider, ISpecification holder) {
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