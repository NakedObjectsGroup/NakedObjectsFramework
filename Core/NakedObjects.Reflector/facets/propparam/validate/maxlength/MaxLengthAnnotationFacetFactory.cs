// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.ComponentModel.DataAnnotations;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Propparam.Validate.MaxLength;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;
using MemberInfo = System.Reflection.MemberInfo;
using MethodInfo = System.Reflection.MethodInfo;
using PropertyInfo = System.Reflection.PropertyInfo;
using ParameterInfo = System.Reflection.ParameterInfo;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.MaxLength {
    public class MaxLengthAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public MaxLengthAnnotationFacetFactory(INakedObjectReflector reflector)
            :base(reflector, NakedObjectFeatureType.ObjectsPropertiesAndParameters) { }

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            Attribute attribute = type.GetCustomAttributeByReflection<StringLengthAttribute>() ?? (Attribute) type.GetCustomAttributeByReflection<MaxLengthAttribute>();
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        private static bool Process(MemberInfo member, IFacetHolder holder) {
            Attribute attribute = member.GetCustomAttribute<StringLengthAttribute>() ?? (Attribute) member.GetCustomAttribute<MaxLengthAttribute>();

            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, IFacetHolder holder) {
            return Process(method, holder);
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, IFacetHolder holder) {
            return Process(property, holder);
        }

        public override bool ProcessParams(MethodInfo method, int paramNum, IFacetHolder holder) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            Attribute attribute = parameter.GetCustomAttributeByReflection<StringLengthAttribute>() ?? (Attribute) parameter.GetCustomAttributeByReflection<MaxLengthAttribute>();

            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        private static IMaxLengthFacet Create(Attribute attribute, IFacetHolder holder) {
            if (attribute == null) {
                return null;
            }
            if (attribute is StringLengthAttribute) {
                return Create((StringLengthAttribute) attribute, holder);
            }
            if (attribute is MaxLengthAttribute) {
                return Create((MaxLengthAttribute) attribute, holder);
            }

            throw new ArgumentException("Unexpected attribute type: " + attribute.GetType());
        }

        private static IMaxLengthFacet Create(MaxLengthAttribute attribute, IFacetHolder holder) {
            return attribute == null ? null : new MaxLengthFacetAnnotation(attribute.Length, holder);
        }


        private static IMaxLengthFacet Create(StringLengthAttribute attribute, IFacetHolder holder) {
            return attribute == null ? null : new MaxLengthFacetAnnotation(attribute.MaximumLength, holder);
        }
    }
}