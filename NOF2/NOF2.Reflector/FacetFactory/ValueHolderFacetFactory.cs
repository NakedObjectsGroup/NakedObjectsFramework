// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.SemanticsProvider;
using NakedFramework.Metamodel.Utils;
using NakedFramework.ParallelReflector.TypeFacetFactory;
using NOF2.Reflector.Helpers;
using NOF2.Reflector.SemanticsProvider;
using NOF2.ValueHolder;

namespace NOF2.Reflector.FacetFactory;

public sealed class ValueHolderFacetFactory : ValueUsingValueSemanticsProviderFacetFactory {
    public ValueHolderFacetFactory(IFacetFactoryOrder<ValueHolderFacetFactory> order, ILoggerFactory loggerFactory) : base(order.Order, loggerFactory) { }

    private static IList<IFacet> GetFacets(Type type, object sm) =>
        typeof(ValueHolderFacetFactory).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).Where(m => m.Name is "GetParserFacet" or "GetTitleFacet" or "GetValueFacet").Select(m => m.MakeGenericMethod(type)).Select(im => im.Invoke(null, new[] { sm })).Cast<IFacet>().ToList();

    private static IFacet GetMaskFacet(Type type, Type valueType) =>
        typeof(ValueHolderFacetFactory).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).Where(m => m.Name is "GetMaskFacet" && m.IsGenericMethod).Select(m => m.MakeGenericMethod(type, valueType)).Select(im => im.Invoke(null, null)).Cast<IFacet>().SingleOrDefault();

    private static void AddFacets(ISpecificationBuilder holder, Type type, Type valueType) {
        var semanticsProviderType = typeof(ValueHolderWrappingValueSemanticsProvider<,>).MakeGenericType(type, valueType);
        var semanticsProvider = Activator.CreateInstance(semanticsProviderType);

        var facets = GetFacets(type, semanticsProvider);

        facets.Add(GetMaskFacet(type, valueType));
        facets.Add(new TypeFacet(valueType));
        facets.Add(NotPersistedFacet.Instance);
        facets.Add(AggregatedFacetAlways.Instance);

        FacetUtils.AddFacets(facets, holder);
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var valueType = NOF2Helpers.IsOrImplementsValueHolder(type);

        if (valueType is not null) {
            var (oSpec, mm) = reflector.LoadSpecification<IObjectSpecImmutable>(type, metamodel);
            AddFacets(specification, type, valueType);
            return mm;
        }

        return metamodel;
    }

    // Used via reflection below - do not remove 
    // ReSharper disable UnusedMember.Local
    private static IFacet GetParserFacet<T>(IValueSemanticsProvider<T> sm) => new ParseableFacetUsingParser<T>(sm);

    private static IFacet GetTitleFacet<T>(IValueSemanticsProvider<T> sm) => new TitleFacetUsingParser<T>(sm);

    private static IFacet GetValueFacet<T>(IValueSemanticsProvider<T> sm) => new ValueFacetFromSemanticProvider<T>(sm);

    private static IFacet GetMaskFacet<T, TU>() where T : class, IValueHolder<TU>, new() {
        var vh = new T();
        var mask = vh.Mask;
        return mask is null ? null : new MaskFacet(mask);
    }
    // ReSharper restore UnusedMember.Local
}