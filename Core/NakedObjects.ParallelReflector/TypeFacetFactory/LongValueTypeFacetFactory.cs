// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.SemanticsProvider;

namespace NakedObjects.ParallelReflect.TypeFacetFactory {
    public sealed class LongValueTypeFacetFactory : ValueUsingValueSemanticsProviderFacetFactory {
        public LongValueTypeFacetFactory(int numericOrder) : base(numericOrder) { }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (LongValueSemanticsProvider.IsAdaptedType(type)) {
                var (oSpec, mm) = reflector.LoadSpecification(LongValueSemanticsProvider.AdaptedType, metamodel);

                metamodel = mm;
                var spec = oSpec as IObjectSpecImmutable;
                AddValueFacets(new LongValueSemanticsProvider(spec, specification), specification);
            }

            return metamodel;
        }
    }
}