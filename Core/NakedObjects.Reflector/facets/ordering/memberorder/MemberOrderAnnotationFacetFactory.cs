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
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Ordering.MemberOrder {
    public class MemberOrderAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public MemberOrderAnnotationFacetFactory(IReflector reflector)
            : base(reflector, FeatureType.PropertiesCollectionsAndActions) {}

        private static bool Process(MemberInfo member, ISpecification holder) {
            var attribute = AttributeUtils.GetCustomAttribute<MemberOrderAttribute>(member);
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecification specification) {
            return Process(method, specification);
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, ISpecification specification) {
            return Process(property, specification);
        }

        private static IMemberOrderFacet Create(MemberOrderAttribute attribute, ISpecification holder) {
            return attribute == null ? null : new MemberOrderFacetAnnotation(attribute.Name, attribute.Sequence, holder);
        }
    }
}