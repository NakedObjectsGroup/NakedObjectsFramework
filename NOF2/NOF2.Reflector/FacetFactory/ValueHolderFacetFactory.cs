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

    private static IFacet GetParserFacet<T>(IValueSemanticsProvider<T> sm, ISpecificationBuilder holder) => new ParseableFacetUsingParser<T>(sm, holder);

    private static IFacet GetTitleFacet<T>(IValueSemanticsProvider<T> sm, ISpecificationBuilder holder) => new TitleFacetUsingParser<T>(sm, holder);

    private static IFacet GetValueFacet<T>(IValueSemanticsProvider<T> sm, ISpecificationBuilder holder) => new ValueFacetFromSemanticProvider<T>(sm, holder);

    private static IFacet GetMaskFacet<T, TU>(ISpecificationBuilder holder) where T : class, IValueHolder<TU>, new() {
        var vh = new T();
        var mask = vh.Mask;
        return mask is null ? null : new MaskFacet(mask, holder);
    }

    private static IEnumerable<IFacet> GetFacets(Type type, object sm, ISpecificationBuilder holder) =>
        typeof(ValueHolderFacetFactory).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).Where(m => m.Name is "GetParserFacet" or "GetTitleFacet" or "GetValueFacet").Select(m => m.MakeGenericMethod(type)).Select(im => im.Invoke(null, new[] { sm, holder })).Cast<IFacet>();

    private static IFacet GetMaskFacet(Type type, Type valueType, ISpecificationBuilder holder) =>
        typeof(ValueHolderFacetFactory).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).Where(m => m.Name is "GetMaskFacet" && m.IsGenericMethod).Select(m => m.MakeGenericMethod(type, valueType)).Select(im => im.Invoke(null, new object[] { holder })).Cast<IFacet>().SingleOrDefault();

    private static void AddFacets(ISpecificationBuilder holder, Type type, Type valueType, IObjectSpecImmutable spec) {
        var semanticsProviderType = typeof(ValueHolderWrappingValueSemanticsProvider<,>).MakeGenericType(type, valueType);
        var semanticsProvider = Activator.CreateInstance(semanticsProviderType, spec, holder);

        foreach (var facet in GetFacets(type, semanticsProvider, holder)) {
            FacetUtils.AddFacet(facet);
        }

        FacetUtils.AddFacet(GetMaskFacet(type, valueType, holder));

        FacetUtils.AddFacet(new TypeFacet(holder, valueType));
        FacetUtils.AddFacet(new NotPersistedFacet(holder));
        FacetUtils.AddFacet(new AggregatedFacetAlways(holder));
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var valueType = NOF2Helpers.IsOrImplementsValueHolder(type);

        if (valueType is not null) {
            var (oSpec, mm) = reflector.LoadSpecification<IObjectSpecImmutable>(type, metamodel);
            AddFacets(specification, type, valueType, oSpec);
            return mm;
        }

        return metamodel;
    }
}