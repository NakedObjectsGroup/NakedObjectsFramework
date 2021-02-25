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
using NakedObjects.Meta.SemanticsProvider;

namespace NakedObjects.Reflector.TypeFacetFactory {
    public sealed class ULongValueTypeFacetFactory : ValueUsingValueSemanticsProviderFacetFactory {
        public ULongValueTypeFacetFactory(IFacetFactoryOrder<ULongValueTypeFacetFactory> order, ILoggerFactory loggerFactory) : base(order.Order, loggerFactory) { }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (!ULongValueSemanticsProvider.IsAdaptedType(type)) {
                return metamodel;
            }

            var (oSpec, mm) = reflector.LoadSpecification<IObjectSpecImmutable>(ULongValueSemanticsProvider.AdaptedType, metamodel);
            AddValueFacets(new ULongValueSemanticsProvider(oSpec, specification), specification);
            return mm;
        }
    }
}