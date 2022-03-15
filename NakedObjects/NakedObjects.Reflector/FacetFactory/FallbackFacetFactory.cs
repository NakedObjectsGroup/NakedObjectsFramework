// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
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

namespace NakedObjects.Reflector.FacetFactory;

/// <summary>
///     Central point for providing some kind of default for any  <see cref="IFacet" />s required by the Naked Objects
///     Framework itself.
/// </summary>
public sealed class FallbackFacetFactory : DomainObjectFacetFactoryProcessor {
    public FallbackFacetFactory(IFacetFactoryOrder<FallbackFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.Everything) { }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var namedFacet = new NamedFacetInferred(type.Name);
        var pluralName = NameUtils.PluralName(namedFacet.FriendlyName);
        var pluralFacet = new PluralFacetInferred(pluralName);

        FacetUtils.AddFacets(
            new IFacet[] {
                DescribedAsFacetNone.Instance,
                ImmutableFacetNever.Instance,
                TitleFacetNone.Instance,
                namedFacet,
                pluralFacet,
                ValueFacet.Instance,
                SaveFacet.Instance
            }, specification);
        return metamodel;
    }

    private static void Process(ISpecificationBuilder holder) {
        var facets = new List<IFacet>();

        if (holder is IMemberSpecImmutable specImmutable) {
            facets.Add(new MemberNamedFacetInferred(specImmutable.Identifier.MemberName));
            facets.Add(DescribedAsFacetNone.Instance);
        }

        if (holder is IAssociationSpecImmutable) {
            facets.Add(ImmutableFacetNever.Instance);
            facets.Add(PropertyDefaultFacetNone.Instance);
            facets.Add(PropertyValidateFacetNone.Instance);
        }

        if (holder is IOneToOneAssociationSpecImmutable immutable) {
            facets.Add(MaxLengthFacetZero.Instance);
            facets.Add(MultiLineFacetNone.Instance);
        }

        if (holder is IActionSpecImmutable) {
            facets.Add(ActionDefaultsFacetNone.Instance);
            facets.Add(ActionChoicesFacetNone.Instance);
            facets.Add(PageSizeFacetDefault.Instance);
        }

        FacetUtils.AddFacets(facets, holder);
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        Process(specification);
        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        Process(specification);
        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var facets = new List<IFacet>();

        if (holder is IActionParameterSpecImmutable param) {
            var name = method.GetParameters()[paramNum].Name ?? method.GetParameters()[paramNum].ParameterType.FullName;
            facets.Add(new MemberNamedFacetInferred(name));
            facets.Add(DescribedAsFacetNone.Instance);
            facets.Add(MultiLineFacetNone.Instance);
            facets.Add(MaxLengthFacetZero.Instance);
        }

        FacetUtils.AddFacets(facets, holder);
        return metamodel;
    }
}

// Copyright (c) Naked Objects Group Ltd.