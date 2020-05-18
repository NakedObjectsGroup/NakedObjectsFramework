// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Reflect.FacetFactory {
    /// <summary>
    ///     Simply installs a <see cref="MandatoryFacetDefault" /> onto all properties and parameters.
    /// </summary>
    /// <para>
    ///     The idea is that this <see cref="IFacetFactory" /> is included early on in the
    ///     <see cref="FacetFactorySet" />, but other <see cref="IMandatoryFacet" /> implementations
    ///     which don't require mandatory semantics will potentially replace these where the
    ///     property or parameter is annotated or otherwise indicated as being optional.
    /// </para>
    public sealed class MandatoryDefaultFacetFactory : FacetFactoryAbstract {
        public MandatoryDefaultFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.PropertiesAndActionParameters) { }

        public override void Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification) => FacetUtils.AddFacet(Create(specification));

        public override void Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification) => FacetUtils.AddFacet(Create(specification));

        public override void ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder) => FacetUtils.AddFacet(Create(holder));

        private static IMandatoryFacet Create(ISpecification holder) => new MandatoryFacetDefault(holder);
    }
}