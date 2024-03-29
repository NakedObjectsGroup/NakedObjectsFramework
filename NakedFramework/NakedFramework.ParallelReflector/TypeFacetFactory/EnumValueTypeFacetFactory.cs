// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Metamodel.SemanticsProvider;

namespace NakedFramework.ParallelReflector.TypeFacetFactory;

public sealed class EnumValueTypeFacetFactory : ValueUsingValueSemanticsProviderFacetFactory {
    public EnumValueTypeFacetFactory(IFacetFactoryOrder<EnumValueTypeFacetFactory> order, ILoggerFactory loggerFactory) : base(order.Order, loggerFactory) { }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        if (!typeof(Enum).IsAssignableFrom(type)) {
            return metamodel;
        }

        var semanticsProviderType = typeof(EnumValueSemanticsProvider<>).MakeGenericType(type);
        var (oSpec, mm) = reflector.LoadSpecification<IObjectSpecImmutable>(type, metamodel);
        if (Activator.CreateInstance(semanticsProviderType) is IValueSemanticsProvider semanticsProvider) {
            semanticsProvider.AddValueFacets(specification);
        }
        else {
            throw new UnknownTypeException($"Created type {semanticsProviderType} is not IValueSemanticsProvider");
        }

        return mm;
    }
}