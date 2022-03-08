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
using NOF2.Attribute;
using NOF2.Reflector.Helpers;

namespace NOF2.Reflector.FacetFactory;

public sealed class NamedAnnotationFacetFactory : AbstractNOF2FacetFactoryProcessor, IAnnotationBasedFacetFactory {
    public NamedAnnotationFacetFactory(IFacetFactoryOrder<NamedAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.Everything) { }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var attribute = type.GetCustomAttribute<INamedAttribute>();
        FacetUtils.AddFacet(Create(attribute, specification), specification);
        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var attribute = method.GetCustomAttribute<INamedAttribute>();
        FacetUtils.AddFacet(CreateForMember(attribute, specification), specification);
        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var attribute = property.GetCustomAttribute<INamedAttribute>();
        FacetUtils.AddFacet(CreateForMember(attribute, specification), specification);
        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var parameter = method.GetParameters()[paramNum];
        var attribute = parameter.GetCustomAttribute<INamedAttribute>();
        FacetUtils.AddFacet(CreateForMember(attribute, holder), holder);
        return metamodel;
    }

    private static INamedFacet Create(INamedAttribute attribute, ISpecification holder) =>
        string.IsNullOrWhiteSpace(attribute?.Name) ? null : new NamedFacetAnnotation(attribute.Name);

    private static IMemberNamedFacet CreateForMember(INamedAttribute attribute, ISpecification holder) =>
        string.IsNullOrWhiteSpace(attribute?.Name) ? null : new MemberNamedFacetAnnotation(attribute.Name);
}