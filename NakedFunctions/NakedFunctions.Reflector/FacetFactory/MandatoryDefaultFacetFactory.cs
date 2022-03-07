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
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;

namespace NakedFunctions.Reflector.FacetFactory;

/// <summary>
///     Simply installs a <see cref="MandatoryFacetDefault" /> onto all properties and parameters.
/// </summary>
/// <para>
///     The idea is that this <see cref="IFacetFactory" /> is included early on in the
///     <see cref="ObjectFacetFactorySet" />, but other <see cref="IMandatoryFacet" /> implementations
///     which don't require mandatory semantics will potentially replace these where the
///     property or parameter is annotated or otherwise indicated as being optional.
/// </para>
public sealed class MandatoryDefaultFacetFactory : FunctionalFacetFactoryProcessor {
    public MandatoryDefaultFacetFactory(IFacetFactoryOrder<MandatoryDefaultFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.PropertiesAndActionParameters) { }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        FacetUtils.AddFacet(new MandatoryFacetDefault(specification), specification);
        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        FacetUtils.AddFacet(new MandatoryFacetDefault(specification), specification);
        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        FacetUtils.AddFacet(new MandatoryFacetDefault(holder), holder);
        return metamodel;
    }
}