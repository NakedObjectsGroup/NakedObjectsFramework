// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;
using NakedFramework.ParallelReflector.Utils;

namespace NakedObjects.Reflector.FacetFactory;

public sealed class RegExAnnotationFacetFactory : DomainObjectFacetFactoryProcessor, IAnnotationBasedFacetFactory {
    private readonly ILogger<RegExAnnotationFacetFactory> logger;

    public RegExAnnotationFacetFactory(IFacetFactoryOrder<RegExAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.ObjectsInterfacesPropertiesAndActionParameters) =>
        logger = loggerFactory.CreateLogger<RegExAnnotationFacetFactory>();

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var attribute = type.GetCustomAttribute<RegularExpressionAttribute>() ?? (Attribute)type.GetCustomAttribute<RegExAttribute>();
        FacetUtils.AddFacet(Create(attribute), specification);
        return metamodel;
    }

    private void Process(MemberInfo member, ISpecificationBuilder holder) {
        var attribute = member.GetCustomAttribute<RegularExpressionAttribute>() ?? (Attribute)member.GetCustomAttribute<RegExAttribute>();
        FacetUtils.AddFacet(Create(attribute), holder);
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        if (TypeUtils.IsString(method.ReturnType)) {
            Process(method, specification);
        }

        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        if (property.HasPublicGetter() && TypeUtils.IsString(property.PropertyType)) {
            Process(property, specification);
        }

        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var parameter = method.GetParameters()[paramNum];
        if (TypeUtils.IsString(parameter.ParameterType)) {
            var attribute = parameter.GetCustomAttribute<RegularExpressionAttribute>() ?? (Attribute)parameter.GetCustomAttribute<RegExAttribute>();
            FacetUtils.AddFacet(Create(attribute), holder);
        }

        return metamodel;
    }

    private IRegExFacet Create(Attribute attribute) =>
        attribute switch {
            null => null,
            RegularExpressionAttribute expressionAttribute => Create(expressionAttribute),
            RegExAttribute exAttribute => Create(exAttribute),
            _ => throw new ArgumentException(logger.LogAndReturn($"Unexpected attribute type: {attribute.GetType()}"))
        };

    private static IRegExFacet Create(RegExAttribute attribute) => new RegExFacet(attribute.Validation, attribute.Format, attribute.CaseSensitive, attribute.Message);

    private static IRegExFacet Create(RegularExpressionAttribute attribute) => new RegExFacet(attribute.Pattern, string.Empty, true, attribute.ErrorMessage);
}