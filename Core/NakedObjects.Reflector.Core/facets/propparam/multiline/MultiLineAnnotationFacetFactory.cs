// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Propparam.MultiLine;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;
using MemberInfo = System.Reflection.MemberInfo;
using MethodInfo = System.Reflection.MethodInfo;
using PropertyInfo = System.Reflection.PropertyInfo;
using ParameterInfo = System.Reflection.ParameterInfo;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.MultiLine {
    public class MultiLineAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public MultiLineAnnotationFacetFactory(INakedObjectReflector reflector)
            : base(reflector, NakedObjectFeatureType.ObjectsPropertiesAndParameters) { }

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            var attribute = type.GetCustomAttributeByReflection<MultiLineAttribute>();
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        private static bool Process(MemberInfo member, IFacetHolder holder) {
            var attribute = member.GetCustomAttribute<MultiLineAttribute>();
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, IFacetHolder holder) {
            if (TypeUtils.IsString(method.ReturnType)) {
                return Process(method, holder);
            }
            return false;
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, IFacetHolder holder) {
            if (property.GetGetMethod() != null && TypeUtils.IsString(property.PropertyType)) {
                return Process(property, holder);
            }
            return false;
        }

        public override bool ProcessParams(MethodInfo method, int paramNum, IFacetHolder holder) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            if (TypeUtils.IsString(parameter.ParameterType)) {
                var attribute = parameter.GetCustomAttributeByReflection<MultiLineAttribute>();
                return FacetUtils.AddFacet(Create(attribute, holder));
            }
            return false;
        }

        private static IMultiLineFacet Create(MultiLineAttribute attribute, IFacetHolder holder) {
            return (attribute != null) ? new MultiLineFacetAnnotation(attribute.NumberOfLines, attribute.Width, holder) : null;
        }
    }
}