// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.ComponentModel.DataAnnotations;

using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Propparam.Validate.RegEx;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;
using MemberInfo = System.Reflection.MemberInfo;
using MethodInfo = System.Reflection.MethodInfo;
using PropertyInfo = System.Reflection.PropertyInfo;
using ParameterInfo = System.Reflection.ParameterInfo;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.RegEx {
    public class RegExAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public RegExAnnotationFacetFactory(INakedObjectReflector reflector)
            :base(reflector, FeatureType.ObjectsPropertiesAndParameters) { }


        public override bool Process(Type type, IMethodRemover methodRemover, ISpecification specification) {
            Attribute attribute = type.GetCustomAttributeByReflection<RegularExpressionAttribute>();
            if (attribute == null) {
                attribute = type.GetCustomAttributeByReflection<RegExAttribute>();
            }
            return FacetUtils.AddFacet(Create(attribute, specification));
        }

        private static bool Process(MemberInfo member, ISpecification holder) {
            Attribute attribute = member.GetCustomAttribute<RegularExpressionAttribute>();
            if (attribute == null) {
                attribute = member.GetCustomAttribute<RegExAttribute>();
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