// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;

namespace NakedObjects.Reflector.FacetFactory;

/// <summary>
///     Creates an <see cref="IQueryOnlyFacet" /> or <see cref="IIdempotentFacet" />  based on the presence of a
///     <see cref="QueryOnlyAttribute" /> or <see cref="IdempotentAttribute" /> annotation
/// </summary>
public sealed class PotencyAnnotationFacetFactory : DomainObjectFacetFactoryProcessor, IAnnotationBasedFacetFactory {
    public PotencyAnnotationFacetFactory(IFacetFactoryOrder<PotencyAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.Actions) { }

    private static void Process(MemberInfo member, ISpecificationBuilder holder) {
        // give priority to Idempotent as more restrictive
        if (member.GetCustomAttribute<IdempotentAttribute>() is not null) {
            FacetUtils.AddFacet(IdempotentFacet.Instance, holder);
        }
        else if (member.GetCustomAttribute<QueryOnlyAttribute>() is not null) {
            FacetUtils.AddFacet(QueryOnlyFacet.Instance, holder);
        }
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        Process(method, specification);
        return metamodel;
    }
}