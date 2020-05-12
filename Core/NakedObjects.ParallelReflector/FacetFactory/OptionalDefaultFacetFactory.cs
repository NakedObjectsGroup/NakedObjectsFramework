// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Immutable;
using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.ParallelReflect.FacetFactory {
    /// <summary>
    ///     Installs a <see cref="OptionalFacetDefault" /> onto all properties and parameters if they are non primitive
    ///     and readable.
    /// </summary>
    /// <para>
    ///     This is an alternative to <see cref="MandatoryDefaultFacetFactory" />. Which works the same way but
    ///     makes everything optional by default. Requiring the use of annotations to indicate mandatoryness.
    /// </para>
    public sealed class OptionalDefaultFacetFactory : FacetFactoryAbstract {
        public OptionalDefaultFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.PropertiesAndActionParameters) { }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            FacetUtils.AddFacet(method.ReturnType.IsPrimitive ? CreateMandatory(specification) : CreateOptional(specification));
            return metamodel;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            FacetUtils.AddFacet(property.PropertyType.IsPrimitive || property.GetGetMethod() == null ? CreateMandatory(specification) : CreateOptional(specification));
            return metamodel;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var parameter = method.GetParameters()[paramNum];
            FacetUtils.AddFacet(parameter.ParameterType.IsPrimitive ? CreateMandatory(holder) : CreateOptional(holder));
            return metamodel;
        }

        private static IMandatoryFacet CreateOptional(ISpecification holder) => new OptionalFacetDefault(holder);

        private static IMandatoryFacet CreateMandatory(ISpecification holder) => new MandatoryFacetDefault(holder);
    }
}