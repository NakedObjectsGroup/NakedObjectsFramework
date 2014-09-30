// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Propparam.Validate.Mask;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;
using MemberInfo = System.Reflection.MemberInfo;
using MethodInfo = System.Reflection.MethodInfo;
using PropertyInfo = System.Reflection.PropertyInfo;
using ParameterInfo = System.Reflection.ParameterInfo;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.Mask {
    public class MaskAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public MaskAnnotationFacetFactory(INakedObjectReflector reflector)
            : base(reflector, NakedObjectFeatureType.ObjectsPropertiesAndParameters) { }

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            var attribute = type.GetCustomAttributeByReflection<MaskAttribute>();
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        private static bool Process(MemberInfo member, IFacetHolder holder) {
            var attribute = member.GetCustomAttribute<MaskAttribute>();
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
            var attribute = parameter.GetCustomAttributeByReflection<MaskAttribute>();
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        private static IMaskFacet Create(MaskAttribute attribute, IFacetHolder holder) {
            return attribute != null ? new MaskFacetAnnotation(attribute.Value, holder) : null;
        }
    }
}