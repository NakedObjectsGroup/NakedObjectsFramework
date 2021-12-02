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
///     Creates an <see cref="IPageSizeFacet" /> based on the presence of an
///     <see cref="PageSizeAttribute" /> annotation
/// </summary>
public sealed class PageSizeAnnotationFacetFactory : ObjectFacetFactoryProcessor, IAnnotationBasedFacetFactory {
    public PageSizeAnnotationFacetFactory(IFacetFactoryOrder<PageSizeAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.Actions) { }

    private static void Process(MemberInfo member, ISpecification holder) {
        var attribute = member.GetCustomAttribute<PageSizeAttribute>();
        FacetUtils.AddFacet(Create(attribute, holder));
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        Process(method, specification);
        return metamodel;
    }

    private static IPageSizeFacet Create(PageSizeAttribute attribute, ISpecification holder) => attribute is null ? null : new PageSizeFacetAnnotation(attribute.Value, holder);
}