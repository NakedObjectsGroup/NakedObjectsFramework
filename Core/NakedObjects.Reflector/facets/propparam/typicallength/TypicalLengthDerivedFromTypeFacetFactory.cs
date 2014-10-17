// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Reflector.DotNet.Facets;

namespace NakedObjects.Architecture.Facets.Propparam.TypicalLength {
    public class TypicalLengthDerivedFromTypeFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public TypicalLengthDerivedFromTypeFacetFactory(INakedObjectReflector reflector)
            : base(reflector, FeatureType.PropertiesAndParameters) {}

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, ISpecification specification) {
            return AddFacetDerivedFromTypeIfPresent(specification, property.PropertyType);
        }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecification specification) {
            Type type = method.ReturnType;
            return AddFacetDerivedFromTypeIfPresent(specification, type);
        }

        public override bool ProcessParams(MethodInfo method, int paramNum, ISpecification holder) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            return AddFacetDerivedFromTypeIfPresent(holder, parameter.ParameterType);
        }

        private bool AddFacetDerivedFromTypeIfPresent(ISpecification holder, Type type) {
            ITypicalLengthFacet facet = GetTypicalLengthFacet(type);
            if (facet != null) {
                return FacetUtils.AddFacet(new TypicalLengthFacetDerivedFromType(facet, holder));
            }
            return false;
        }

        private ITypicalLengthFacet GetTypicalLengthFacet(Type type) {
            var paramTypeSpec = Reflector.LoadSpecification(type);
            return paramTypeSpec.GetFacet<ITypicalLengthFacet>();
        }
    }
}