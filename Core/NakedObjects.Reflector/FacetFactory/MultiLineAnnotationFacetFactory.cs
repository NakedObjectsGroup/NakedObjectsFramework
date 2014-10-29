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
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;
using NakedObjects.Util;

namespace NakedObjects.Reflect.FacetFactory {
    public class MultiLineAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public MultiLineAnnotationFacetFactory(IReflector reflector)
            : base(reflector, FeatureType.ObjectsPropertiesAndParameters) {}

        public override bool Process(Type type, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            var attribute = type.GetCustomAttributeByReflection<MultiLineAttribute>();
            return FacetUtils.AddFacet(Create(attribute, specification));
        }

        private static bool Process(MemberInfo member, ISpecification holder) {
            var attribute = AttributeUtils.GetCustomAttribute<MultiLineAttribute>(member);
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            if (TypeUtils.IsString(method.ReturnType)) {
                return Process(method, specification);
            }
            return false;
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            if (property.GetGetMethod() != null && TypeUtils.IsString(property.PropertyType)) {
                return Process(property, specification);
            }
            return false;
        }

        public override bool ProcessParams(MethodInfo method, int paramNum, ISpecificationBuilder holder) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            if (TypeUtils.IsString(parameter.ParameterType)) {
                var attribute = parameter.GetCustomAttributeByReflection<MultiLineAttribute>();
                return FacetUtils.AddFacet(Create(attribute, holder));
            }
            return false;
        }

        private static IMultiLineFacet Create(MultiLineAttribute attribute, ISpecification holder) {
            return (attribute != null) ? new MultiLineFacetAnnotation(attribute.NumberOfLines, attribute.Width, holder) : null;
        }
    }
}