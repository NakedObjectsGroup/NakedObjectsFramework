// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedObjects;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedFunctions.Reflector.FacetFactory {
    /// <summary>
    ///     Creates an <see cref="IQueryOnlyFacet" /> or <see cref="IIdempotentFacet" />  based on the presence of a
    ///     <see cref="QueryOnlyAttribute" /> or <see cref="NakedObjects.IdempotentAttribute" /> annotation
    /// </summary>
    public sealed class PotencyDerivedFromSignatureFacetFactory : FunctionalFacetFactoryProcessor {
        private readonly ILogger<PotencyDerivedFromSignatureFacetFactory> logger;

        public PotencyDerivedFromSignatureFacetFactory(IFacetFactoryOrder<PotencyDerivedFromSignatureFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.Actions) =>
            logger = loggerFactory.CreateLogger<PotencyDerivedFromSignatureFacetFactory>();

        private static bool IsSideEffectFree(Type returnType) => !FacetUtils.IsTuple(returnType) && !returnType.IsAssignableTo(typeof(IContext));

        private static void Process(MemberInfo member, ISpecification holder) {
            if (member is MethodInfo method) {
                if (IsSideEffectFree(method.ReturnType)) {
                    FacetUtils.AddFacet(new QueryOnlyFacet(holder));
                }
            }
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            Process(method, specification);
            return metamodel;
        }
    }
}