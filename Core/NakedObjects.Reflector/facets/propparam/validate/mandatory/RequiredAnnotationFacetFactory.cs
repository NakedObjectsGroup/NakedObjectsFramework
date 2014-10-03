// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.ComponentModel.DataAnnotations;
using Common.Logging;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Propparam.Validate.Mandatory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;
using MemberInfo = System.Reflection.MemberInfo;
using MethodInfo = System.Reflection.MethodInfo;
using PropertyInfo = System.Reflection.PropertyInfo;
using ParameterInfo = System.Reflection.ParameterInfo;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.Mandatory {
    public class RequiredAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        private static readonly ILog Log = LogManager.GetLogger(typeof (RequiredAnnotationFacetFactory));

        public RequiredAnnotationFacetFactory(INakedObjectReflector reflector)
            :base(reflector, NakedObjectFeatureType.PropertiesAndParameters) { }

        private static bool Process(MemberInfo member, IFacetHolder holder) {
            var attribute = member.GetCustomAttribute<RequiredAttribute>();
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, IFacetHolder holder) {
            return Process(method, holder);
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, IFacetHolder holder) {
            if (property.GetGetMethod() != null) {
                return Process(property, holder);
            }
            return false;
        }

        public override bool ProcessParams(MethodInfo method, int paramNum, IFacetHolder holder) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            var attribute = parameter.GetCustomAttributeByReflection<RequiredAttribute>();
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        private static IMandatoryFacet Create(RequiredAttribute attribute, IFacetHolder holder) {
            return attribute != null ? new MandatoryFacet(holder) : null;
        }
    }
}