// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.DotNet.Facets.Objects.Value;

namespace NakedObjects.Reflector.DotNet.Value {
    public class EnumValueTypeFacetFactory : FacetFactoryAbstract {
        public EnumValueTypeFacetFactory(INakedObjectReflector reflector)
            : base(reflector, NakedObjectFeatureType.ObjectsOnly) {}

        public override bool Process(Type type, IMethodRemover methodRemover, ISpecification specification) {
            if (typeof (Enum).IsAssignableFrom(type)) {
                Type semanticsProviderType = typeof (EnumValueSemanticsProvider<>).MakeGenericType(type);
                var spec = Reflector.LoadSpecification(type);

                object semanticsProvider = Activator.CreateInstance(semanticsProviderType, spec, specification);
                Type facetType = typeof (ValueFacetUsingSemanticsProvider<>).MakeGenericType(type);
                var facet = (IFacet) Activator.CreateInstance(facetType, semanticsProvider, semanticsProvider);
                FacetUtils.AddFacet(facet);
                return true;
            }
            return false;
        }
    }
}