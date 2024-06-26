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

public sealed class UrlLinkAnnotationFacetFactory : FunctionalFacetFactoryProcessor, IAnnotationBasedFacetFactory {
    public UrlLinkAnnotationFacetFactory(IFacetFactoryOrder<UrlLinkAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.PropertiesCollectionsAndActions) { }

    private void Process(MemberInfo member, ISpecificationBuilder holder, Type memberType) {
        var attribute = member.GetCustomAttribute<UrlLinkAttribute>();
        if (attribute is not null && memberType != typeof(string)) {
            Logger<UrlLinkAnnotationFacetFactory>().LogWarning($"UrlLink attribute ignored on non string member: {member.Name}");
            return;
        }

        FacetUtils.AddFacet(Create(attribute), holder);
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        Process(property, specification, property.PropertyType);
        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        Process(method, specification, method.ReturnType);
        return metamodel;
    }

    private static IUrlLinkFacet Create(UrlLinkAttribute attribute) => attribute is not null ? new UrlLinkFacet(attribute.DisplayAs, attribute.AlwaysOpenInNewTab) : null;
}