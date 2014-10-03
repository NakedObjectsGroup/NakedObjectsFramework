// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.ComponentModel;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Naming.DescribedAs;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;
using MemberInfo = System.Reflection.MemberInfo;
using MethodInfo = System.Reflection.MethodInfo;
using PropertyInfo = System.Reflection.PropertyInfo;
using ParameterInfo = System.Reflection.ParameterInfo;

namespace NakedObjects.Reflector.DotNet.Facets.Naming.DescribedAs {
    public class DescribedAsAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public DescribedAsAnnotationFacetFactory(INakedObjectReflector reflector)
            :base(reflector, NakedObjectFeatureType.Everything) { }

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            Attribute attribute = type.GetCustomAttributeByReflection<DescriptionAttribute>() ?? (Attribute) type.GetCustomAttributeByReflection<DescribedAsAttribute>();
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        private static bool Process(MemberInfo member, IFacetHolder holder) {
            Attribute attribute = member.GetCustomAttribute<DescriptionAttribute>();
            if (attribute == null) {
                attribute = member.GetCustomAttribute<DescribedAsAttribute>();
            }
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
            Attribute attribute = parameter.GetCustomAttributeByReflection<DescriptionAttribute>();
            if (attribute == null) {
                attribute = parameter.GetCustomAttributeByReflection<DescribedAsAttribute>();
            }
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        private static IDescribedAsFacet Create(Attribute attribute, IFacetHolder holder) {
            if (attribute == null) {
                return null;
            }
            if (attribute is DescribedAsAttribute) {
                return Create((DescribedAsAttribute) attribute, holder);
            }
            if (attribute is DescriptionAttribute) {
                return Create((DescriptionAttribute) attribute, holder);
            }
            throw new ArgumentException("Unexpected attribute type: " + attribute.GetType());
        }

        private static IDescribedAsFacet Create(DescribedAsAttribute attribute, IFacetHolder holder) {
            return new DescribedAsFacetAnnotation(attribute.Value, holder);
        }

        private static IDescribedAsFacet Create(DescriptionAttribute attribute, IFacetHolder holder) {
            return new DescribedAsFacetAnnotation(attribute.Description, holder);
        }
    }
}