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
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;
using NakedObjects.ParallelReflect.FacetFactory;

namespace NakedObjects.ParallelReflect.FunctionalFacetFactory {
    /// <summary>
    ///     Creates an <see cref="IQueryOnlyFacet" /> or <see cref="IIdempotentFacet" />  based on the presence of a
    ///     <see cref="QueryOnlyAttribute" /> or <see cref="IdempotentAttribute" /> annotation
    /// </summary>
    public sealed class PotencyDerivedFromSignatureFacetFactory : FacetFactoryAbstract {
        private readonly ILogger<PotencyDerivedFromSignatureFacetFactory> logger;

        public PotencyDerivedFromSignatureFacetFactory(IFacetFactoryOrder<PotencyDerivedFromSignatureFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.Actions, ReflectionType.Functional) =>
            logger = loggerFactory.CreateLogger<PotencyDerivedFromSignatureFacetFactory>();


        private static bool TypeIncludesUpdate(Type type) =>
            type switch {
                _ when FacetUtils.IsTuple(type) => TupleIncludesUpdates(type, false),
                _ when FacetUtils.IsAction(type) => false,
                _ => true
            };

        private static bool TupleIncludesUpdates(Type tuple, bool skipFirst) => tuple.GetGenericArguments().Skip(skipFirst ? 1 : 0).Any(TypeIncludesUpdate);

        private static bool IsSideEffectFree(Type returnType) => !FacetUtils.IsTuple(returnType) || !TupleIncludesUpdates(returnType, true);

        private static void Process(MemberInfo member, ISpecification holder) {
            if (member is MethodInfo method) {
                if (IsSideEffectFree(method.ReturnType)) {
                    FacetUtils.AddFacet(new QueryOnlyFacet(holder));
                }
            }
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, IClassStrategy classStrategy, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            Process(method, specification);
            return metamodel;
        }
    }
}