// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.RegEx {
    public class RegExAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public RegExAnnotationFacetFactory(INakedObjectReflector reflector)
            : base(reflector, FeatureType.ObjectsPropertiesAndParameters) {}


        public override bool Process(Type type, IMethodRemover methodRemover, ISpecification specification) {
            Attribute attribute = type.GetCustomAttributeByReflection<RegularExpressionAttribute>();
            if (attribute == null) {
                attribute = type.GetCustomAttributeByReflection<RegExAttribute>();
            }
            return FacetUtils.AddFacet(Create(attribute, specification));
        }

        private static bool Process(MemberInfo member, ISpecification holder) {
            Attribute attribute = AttributeUtils.GetCustomAttribute<RegularExpressionAttribute>(member);
            if (attribute == null) {
                attribute = AttributeUtils.GetCustomAttribute<RegExAttribute>(member);
            }
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecification specification) {
            if (TypeUtils.IsString(method.ReturnType)) {
                return Process(method, specification);
            }
            return false;
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, ISpecification specification) {
            if (property.GetGetMethod() != null && TypeUtils.IsString(property.PropertyType)) {
                return Process(property, specification);
            }
            return false;
        }


        public override bool ProcessParams(MethodInfo method, int paramNum, ISpecification holder) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            if (TypeUtils.IsString(parameter.ParameterType)) {
                Attribute attribute = parameter.GetCustomAttributeByReflection<RegularExpressionAttribute>();
                if (attribute == null) {
                    attribute = parameter.GetCustomAttributeByReflection<RegExAttribute>();
                }

                return FacetUtils.AddFacet(Create(attribute, holder));
            }
            return false;
        }

        private static IRegExFacet Create(Attribute attribute, ISpecification holder) {
            if (attribute == null) {
                return null;
            }
            if (attribute is RegularExpressionAttribute) {
                return Create((RegularExpressionAttribute) attribute, holder);
            }
            if (attribute is RegExAttribute) {
                return Create((RegExAttribute) attribute, holder);
            }
            throw new ArgumentException("Unexpected attribute type: " + attribute.GetType());
        }


        private static IRegExFacet Create(RegExAttribute attribute, ISpecification holder) {
            return new RegExFacetAnnotation(attribute.Validation, attribute.Format, attribute.CaseSensitive, attribute.Message, holder);
        }

        private static IRegExFacet Create(RegularExpressionAttribute attribute, ISpecification holder) {
            return new RegExFacetAnnotation(attribute.Pattern, string.Empty, true, attribute.ErrorMessage, holder);
        }
    }
}