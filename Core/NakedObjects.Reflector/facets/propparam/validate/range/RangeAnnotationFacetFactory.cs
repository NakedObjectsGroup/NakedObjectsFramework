// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.ComponentModel.DataAnnotations;
using Common.Logging;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Propparam.Validate.Range;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;
using MemberInfo = System.Reflection.MemberInfo;
using MethodInfo = System.Reflection.MethodInfo;
using PropertyInfo = System.Reflection.PropertyInfo;
using ParameterInfo = System.Reflection.ParameterInfo;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.Range {
    public class RangeAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        private static readonly ILog Log = LogManager.GetLogger(typeof (RangeAnnotationFacetFactory));

        public RangeAnnotationFacetFactory(INakedObjectReflector reflector)
            :base(reflector, FeatureType.PropertiesAndParameters) { }


        private static bool Process(MemberInfo member, bool isDate, ISpecification specification) {
            var attribute = member.GetCustomAttribute<RangeAttribute>();
            return FacetUtils.AddFacet(Create(attribute, isDate, specification));
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, ISpecification specification) {
            bool isDate = property.PropertyType.IsAssignableFrom(typeof (DateTime));
            return Process(property, isDate, specification);
        }

        public override bool ProcessParams(MethodInfo method, int paramNum, ISpecification holder) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            bool isDate = parameter.ParameterType.IsAssignableFrom(typeof (DateTime));
            var range = parameter.GetCustomAttributeByReflection<RangeAttribute>();
            return FacetUtils.AddFacet(Create(range, isDate, holder));
        }

        private static IRangeFacet Create(RangeAttribute attribute, bool isDate, ISpecification holder) {
            if (attribute != null && attribute.OperandType != typeof (int) && attribute.OperandType != typeof (double)) {
                Log.WarnFormat("Unsupported use of range attribute with explicit type on {0}", holder);
                return null;
            }
            return attribute == null ? null : new RangeFacetAnnotation(attribute.Minimum, attribute.Maximum, isDate, holder);
        }
    }
}