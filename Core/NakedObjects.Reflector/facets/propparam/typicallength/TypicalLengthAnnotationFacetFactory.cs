// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.TypicalLength;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;
using MemberInfo = System.Reflection.MemberInfo;
using MethodInfo = System.Reflection.MethodInfo;
using PropertyInfo = System.Reflection.PropertyInfo;
using ParameterInfo = System.Reflection.ParameterInfo;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.TypicalLength {
    public class TypicalLengthAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public TypicalLengthAnnotationFacetFactory(INakedObjectReflector reflector)
            :base(reflector, FeatureType.ObjectsPropertiesAndParameters) { }

        public override bool Process(Type type, IMethodRemover methodRemover, ISpecification specification) {
            var attribute = type.GetCustomAttributeByReflection<TypicalLengthAttribute>();
            return FacetUtils.AddFacet(Create(attribute, specification));
        }

        private static bool Process(MemberInfo member, ISpecification holder) {
            var attribute = member.GetCustomAttribute<TypicalLengthAttribute>();
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecification specification) {
            return Process(method, specification);
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, ISpecification specification) {
            return Process(property, specification);
        }

        public override bool ProcessParams(MethodInfo method, int paramNum, ISpecification holder) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            var attribute = parameter.GetCustomAttributeByReflection<TypicalLengthAttribute>();
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        private static ITypicalLengthFacet Create(TypicalLengthAttribute attribute, ISpecification holder) {
            return attribute == null ? null : new TypicalLengthFacetAnnotation(attribute.Value, holder);
        }
    }
}