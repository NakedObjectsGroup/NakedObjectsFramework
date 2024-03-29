// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;
using NakedFramework.ParallelReflector.Utils;

namespace NakedObjects.Reflector.FacetFactory;

/// <summary>
///     Creates an <see cref="IFindMenuFacet" /> based on the presence of an
///     <see cref="FindMenuAttribute" /> annotation
/// </summary>
public sealed class FindMenuFacetFactory : DomainObjectFacetFactoryProcessor, IAnnotationBasedFacetFactory {
    private readonly ILogger<FindMenuFacetFactory> logger;

    public FindMenuFacetFactory(IFacetFactoryOrder<FindMenuFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.PropertiesAndActionParameters) =>
        logger = loggerFactory.CreateLogger<FindMenuFacetFactory>();

    private static void Process(MemberInfo member, ISpecificationBuilder holder) {
        var attribute = member.GetCustomAttribute<FindMenuAttribute>();
        FacetUtils.AddFacet(Create(attribute), holder);
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        Process(method, specification);
        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var pType = property.PropertyType;
        if ((pType.IsPrimitive || pType == typeof(string) || TypeUtils.IsEnum(pType)) && property.GetCustomAttribute<FindMenuAttribute>() is not null) {
            logger.LogWarning($"Ignoring FindMenu annotation on primitive or un-readable parameter on {property.ReflectedType}.{property.Name}");
            return metamodel;
        }

        if (property.HasPublicGetter() && !property.PropertyType.IsPrimitive) {
            Process(property, specification);
        }

        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var parameter = method.GetParameters()[paramNum];
        var pType = parameter.ParameterType;
        if (pType.IsPrimitive || pType == typeof(string) || TypeUtils.IsEnum(pType)) {
            if (method.GetCustomAttribute<FindMenuAttribute>() is not null) {
                logger.LogWarning($"Ignoring FindMenu annotation on primitive parameter {paramNum} on {method.ReflectedType}.{method.Name}");
            }

            return metamodel;
        }

        var attribute = parameter.GetCustomAttribute<FindMenuAttribute>();
        FacetUtils.AddFacet(Create(attribute), holder);
        return metamodel;
    }

    private static IFacet Create(FindMenuAttribute attribute) => attribute is null ? null : FindMenuFacet.Instance;
}