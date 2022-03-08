// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;

namespace NakedObjects.Reflector.FacetFactory;

public sealed class NamedAnnotationFacetFactory : DomainObjectFacetFactoryProcessor, IAnnotationBasedFacetFactory {
    private readonly ILogger<NamedAnnotationFacetFactory> logger;

    public NamedAnnotationFacetFactory(IFacetFactoryOrder<NamedAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.Everything) =>
        logger = loggerFactory.CreateLogger<NamedAnnotationFacetFactory>();

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var attribute = type.GetCustomAttribute<DisplayNameAttribute>() ?? (Attribute)type.GetCustomAttribute<NamedAttribute>();
        FacetUtils.AddFacet(Create(attribute), specification);
        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var attribute = method.GetCustomAttribute<DisplayNameAttribute>() ?? (Attribute)method.GetCustomAttribute<NamedAttribute>();
        FacetUtils.AddFacet(CreateForMember(attribute), specification);
        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var attribute = property.GetCustomAttribute<DisplayNameAttribute>() ?? (Attribute)property.GetCustomAttribute<NamedAttribute>();
        FacetUtils.AddFacet(CreateForMember(attribute), specification);
        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var parameter = method.GetParameters()[paramNum];
        var attribute = parameter.GetCustomAttribute<DisplayNameAttribute>() ?? (Attribute)parameter.GetCustomAttribute<NamedAttribute>();
        FacetUtils.AddFacet(CreateForMember(attribute), holder);
        return metamodel;
    }

    private INamedFacet Create(Attribute attribute) =>
        attribute switch {
            null => null,
            NamedAttribute namedAttribute => new NamedFacetAnnotation(namedAttribute.Value),
            DisplayNameAttribute nameAttribute => new NamedFacetAnnotation(nameAttribute.DisplayName),
            _ => throw new ArgumentException(logger.LogAndReturn($"Unexpected attribute type: {attribute.GetType()}"))
        };

    private IMemberNamedFacet CreateForMember(Attribute attribute) =>
        attribute switch {
            null => null,
            NamedAttribute namedAttribute => new MemberNamedFacetAnnotation(namedAttribute.Value),
            DisplayNameAttribute nameAttribute => new MemberNamedFacetAnnotation(nameAttribute.DisplayName),
            _ => throw new ArgumentException(logger.LogAndReturn($"Unexpected attribute type: {attribute.GetType()}"))
        };
}