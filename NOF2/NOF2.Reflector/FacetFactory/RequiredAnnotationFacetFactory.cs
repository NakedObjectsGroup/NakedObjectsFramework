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
using NakedFramework.ParallelReflector.Utils;
using NOF2.Attribute;
using NOF2.Reflector.Helpers;

namespace NOF2.Reflector.FacetFactory;

public sealed class RequiredAnnotationFacetFactory : AbstractNOF2FacetFactoryProcessor, IAnnotationBasedFacetFactory {
    public RequiredAnnotationFacetFactory(IFacetFactoryOrder<RequiredAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.PropertiesAndActionParameters) { }

    private static void Process(MemberInfo member, ISpecificationBuilder holder) {
        var attribute = member.GetCustomAttribute<IRequiredAttribute>();
        FacetUtils.AddFacet(Create(attribute), holder);
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        Process(method, specification);
        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        if (property.HasPublicGetter()) {
            Process(property, specification);
        }

        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var parameter = method.GetParameters()[paramNum];
        var attribute = parameter.GetCustomAttribute<IRequiredAttribute>();
        FacetUtils.AddFacet(Create(attribute), holder);
        return metamodel;
    }

    private static IMandatoryFacet Create(IRequiredAttribute attribute) => attribute?.IsRequired == true ? new MandatoryFacet() : new OptionalFacet();
}