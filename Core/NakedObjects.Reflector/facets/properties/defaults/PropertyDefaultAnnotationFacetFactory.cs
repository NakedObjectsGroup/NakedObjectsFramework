// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.ComponentModel;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Properties.Defaults;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;
using MemberInfo = System.Reflection.MemberInfo;
using PropertyInfo = System.Reflection.PropertyInfo;

namespace NakedObjects.Reflector.DotNet.Facets.Properties.Defaults {
    public class PropertyDefaultAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public PropertyDefaultAnnotationFacetFactory(INakedObjectReflector reflector)
            :base(reflector, NakedObjectFeatureType.PropertiesOnly) { }


        private static bool Process(MemberInfo member, IFacetHolder holder) {
            var attribute = member.GetCustomAttribute<DefaultValueAttribute>();
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, IFacetHolder holder) {
            return Process(property, holder);
        }

        private static IPropertyDefaultFacet Create(DefaultValueAttribute attribute, IFacetHolder holder) {
            return attribute == null ? null : new PropertyDefaultFacetAnnotation(attribute.Value, holder);
        }
    }
}