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
        public HiddenAnnotationFacetFactory()
            : base(NakedObjectFeatureType.PropertiesCollectionsAndActions) {}

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            return Process(type.GetCustomAttributeByReflection<HiddenAttribute>,
                           type.GetCustomAttributeByReflection<ScaffoldColumnAttribute>, holder);
        }

        private static bool Process(MemberInfo member, IFacetHolder holder) {
            return Process(member.GetCustomAttribute<HiddenAttribute>,
                           member.GetCustomAttribute<ScaffoldColumnAttribute>, holder);
        }

        private static bool Process(Func<Attribute> getHidden, Func<Attribute> getScaffold, IFacetHolder holder) {
            Attribute attribute = getHidden();
            if (attribute != null) {
                return FacetUtils.AddFacet(Create((HiddenAttribute) attribute, holder));
            }
            attribute = getScaffold();
            return FacetUtils.AddFacet(Create((ScaffoldColumnAttribute) attribute, holder));
        }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, IFacetHolder holder) {
            return Process(method, holder);
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, IFacetHolder holder) {
            return Process(property, holder);
        }

        private static IHiddenFacet Create(HiddenAttribute attribute, IFacetHolder holder) {
            return attribute == null ? null : new HiddenFacetAnnotation(attribute.Value.ToWhen(), holder);
        }

        private static IHiddenFacet Create(ScaffoldColumnAttribute attribute, IFacetHolder holder) {
            return attribute == null ? null : new HiddenFacetAnnotation(attribute.Scaffold ? When.Never : When.Always, holder);
        }
    }
}