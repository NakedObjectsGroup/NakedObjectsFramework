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
using NakedFramework;
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

public sealed class HiddenAnnotationFacetFactory : AbstractNOF2FacetFactoryProcessor, IAnnotationBasedFacetFactory {
    public HiddenAnnotationFacetFactory(IFacetFactoryOrder<HiddenAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.PropertiesCollectionsAndActions) { }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        Process(type, specification);
        return metamodel;
    }

    private static WhenTo Map(Enum.WhenTo whenTo) =>
        whenTo switch {
            Enum.WhenTo.Always => WhenTo.Always,
            Enum.WhenTo.Never => WhenTo.Never,
            Enum.WhenTo.OncePersisted => WhenTo.OncePersisted,
            Enum.WhenTo.UntilPersisted => WhenTo.UntilPersisted,
            _ => throw new NotImplementedException()
        };

    private static void Process(object onObject, ISpecification specification) {
        var attribute = onObject.GetCustomAttribute<IHiddenAttribute>();
        FacetUtils.AddFacet(Create(attribute, specification));
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        Process(method, specification);
        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        Process(property, specification);
        return metamodel;
    }

    private static IHiddenFacet Create(IHiddenAttribute attribute, ISpecification holder) => attribute is null ? null : new HiddenFacet(Map(attribute.WhenTo), holder);
}