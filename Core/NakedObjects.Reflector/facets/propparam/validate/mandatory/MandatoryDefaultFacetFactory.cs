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
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Propparam.Validate.Mandatory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.Mandatory {
    /// <summary>
    ///     Simply installs a <see cref="MandatoryFacetDefault" /> onto all properties and parameters.
    /// </summary>
    /// <para>
    ///     The idea is that this <see cref="IFacetFactory" /> is included early on in the
    ///     <see cref="FacetFactorySetImpl" />, but other <see cref="IMandatoryFacet" /> implementations
    ///     which don't require mandatory semantics will potentially replace these where the
    ///     property or parameter is annotated or otherwise indicated as being optional.
    /// </para>
    public class MandatoryDefaultFacetFactory : FacetFactoryAbstract {
        public MandatoryDefaultFacetFactory(INakedObjectReflector reflector)
            : base(reflector, FeatureType.PropertiesAndParameters) {}

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecification specification) {
            return FacetUtils.AddFacet(Create(specification));
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, ISpecification specification) {
            return FacetUtils.AddFacet(Create(specification));
        }

        public override bool ProcessParams(MethodInfo method, int paramNum, ISpecification holder) {
            return FacetUtils.AddFacet(Create(holder));
        }

        private static IMandatoryFacet Create(ISpecification holder) {
            return new MandatoryFacetDefault(holder);
        }
    }
}