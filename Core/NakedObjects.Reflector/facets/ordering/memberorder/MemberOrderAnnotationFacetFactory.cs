// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 


using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Ordering.MemberOrder;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;
using MemberInfo = System.Reflection.MemberInfo;
using MethodInfo = System.Reflection.MethodInfo;
using PropertyInfo = System.Reflection.PropertyInfo;

namespace NakedObjects.Reflector.DotNet.Facets.Ordering.MemberOrder {
    public class MemberOrderAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public MemberOrderAnnotationFacetFactory(IMetadata metadata)
            : base(metadata, NakedObjectFeatureType.PropertiesCollectionsAndActions) { }

        private static bool Process(MemberInfo member, IFacetHolder holder) {
            var attribute =  member.GetCustomAttribute<MemberOrderAttribute>();
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, IFacetHolder holder) {
            return Process(method, holder);
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, IFacetHolder holder) {
            return Process(property, holder);
        }

        private static IMemberOrderFacet Create(MemberOrderAttribute attribute, IFacetHolder holder) {
            return attribute == null ? null : new MemberOrderFacetAnnotation(attribute.Name, attribute.Sequence, holder);
        }
    }
}