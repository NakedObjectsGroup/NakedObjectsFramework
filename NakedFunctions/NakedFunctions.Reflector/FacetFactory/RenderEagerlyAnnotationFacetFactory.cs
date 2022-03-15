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
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;

namespace NakedFunctions.Reflector.FacetFactory;

public sealed class RenderEagerlyAnnotationFacetFactory : FunctionalFacetFactoryProcessor, IAnnotationBasedFacetFactory {
    public RenderEagerlyAnnotationFacetFactory(IFacetFactoryOrder<RenderEagerlyAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.EverythingButActionParameters) { }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var attribute = type.GetCustomAttribute<RenderEagerlyAttribute>();
        FacetUtils.AddFacet(Create(attribute), specification);
        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var attribute = property.GetCustomAttribute<RenderEagerlyAttribute>();
        FacetUtils.AddFacet(Create(attribute), specification);
        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var attribute = method.GetCustomAttribute<RenderEagerlyAttribute>();
        FacetUtils.AddFacet(Create(attribute), specification);
        return metamodel;
    }

    private static IEagerlyFacet Create(RenderEagerlyAttribute attribute) => attribute == null ? null : EagerlyFacet.Instance;
}