// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.ParallelReflector.TypeFacetFactory;
using NakedFramework.ParallelReflector.Utils;
using NakedLegacy.Reflector.SemanticsProvider;
using NakedObjects;

namespace NakedLegacy.Reflector.FacetFactory {
    public sealed class TextStringValueTypeFacetFactory : ValueUsingValueSemanticsProviderFacetFactory {
        public TextStringValueTypeFacetFactory(IFacetFactoryOrder<TextStringValueTypeFacetFactory> order, ILoggerFactory loggerFactory) : base(order.Order, loggerFactory) { }

        private static void RemoveExplicitlyIgnoredMembers(Type type, IMethodRemover methodRemover) {
            foreach (var method in type.GetMethods().Where(m => m.GetCustomAttribute<NakedObjectsIgnoreAttribute>() is not null)) {
                methodRemover.SafeRemoveMethod(method);
            }
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (!TextStringValueSemanticsProvider.IsAdaptedType(type)) {
                return metamodel;
            }

            RemoveExplicitlyIgnoredMembers(type, methodRemover);

            var (oSpec, mm) = reflector.LoadSpecification<IObjectSpecImmutable>(TextStringValueSemanticsProvider.AdaptedType, metamodel);
            AddValueFacets(new TextStringValueSemanticsProvider(oSpec, specification), specification);
            return mm;
        }
    }
}