// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 


using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Disable;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;
using MemberInfo = System.Reflection.MemberInfo;
using MethodInfo = System.Reflection.MethodInfo;
using PropertyInfo = System.Reflection.PropertyInfo;

namespace NakedObjects.Reflector.DotNet.Facets.Disable {
    public class DisabledAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public DisabledAnnotationFacetFactory(INakedObjectReflector reflector)
            :base(reflector, NakedObjectFeatureType.PropertiesCollectionsAndActions) { }

        private static bool Process(MemberInfo member, ISpecification holder) {
            var attribute = member.GetCustomAttribute<DisabledAttribute>();
            IDisabledFacet disabledFacet = Create(attribute, holder);
            if (disabledFacet != null) {
                return FacetUtils.AddFacet(disabledFacet);
            }
            return false;
        }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecification specification) {
            return Process(method, specification);
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, ISpecification specification) {
            return Process(property, specification);
        }

        private static IDisabledFacet Create(DisabledAttribute attribute, ISpecification holder) {
            return attribute == null ? null : new DisabledFacetAnnotation(attribute.Value, holder);
        }
    }
}