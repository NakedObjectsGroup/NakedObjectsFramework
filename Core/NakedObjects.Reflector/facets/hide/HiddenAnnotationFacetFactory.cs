// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.ComponentModel.DataAnnotations;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Hide;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;
using MemberInfo = System.Reflection.MemberInfo;
using MethodInfo = System.Reflection.MethodInfo;
using PropertyInfo = System.Reflection.PropertyInfo;

namespace NakedObjects.Reflector.DotNet.Facets.Hide {
    public class HiddenAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public HiddenAnnotationFacetFactory(INakedObjectReflector reflector)
            :base(reflector, FeatureType.PropertiesCollectionsAndActions) { }

        public override bool Process(Type type, IMethodRemover methodRemover, ISpecification specification) {
            return Process(type.GetCustomAttributeByReflection<HiddenAttribute>,
                           type.GetCustomAttributeByReflection<ScaffoldColumnAttribute>, specification);
        }

        private static bool Process(MemberInfo member, ISpecification holder) {
            return Process(member.GetCustomAttribute<HiddenAttribute>,
                           member.GetCustomAttribute<ScaffoldColumnAttribute>, holder);
        }

        private static bool Process(Func<Attribute> getHidden, Func<Attribute> getScaffold, ISpecification specification) {
            Attribute attribute = getHidden();
            if (attribute != null) {
                return FacetUtils.AddFacet(Create((HiddenAttribute) attribute, specification));
            }
            attribute = getScaffold();
            return FacetUtils.AddFacet(Create((ScaffoldColumnAttribute) attribute, specification));
        }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecification specification) {
            return Process(method, specification);
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, ISpecification specification) {
            return Process(property, specification);
        }

        private static IHiddenFacet Create(HiddenAttribute attribute, ISpecification holder) {
            return attribute == null ? null : new HiddenFacetAnnotation(attribute.Value, holder);
        }

        private static IHiddenFacet Create(ScaffoldColumnAttribute attribute, ISpecification holder) {
            return attribute == null ? null : new HiddenFacetAnnotation(attribute.Scaffold ? WhenTo.Never : WhenTo.Always, holder);
        }
    }
}