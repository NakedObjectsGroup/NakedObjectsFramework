// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.ComponentModel;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actions.Defaults;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Actions.Defaults {
    public class ActionDefaultAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public ActionDefaultAnnotationFacetFactory()
            : base(NakedObjectFeatureType.ParametersOnly) {}


        public override bool ProcessParams(MethodInfo method, int paramNum, IFacetHolder holder) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            var attribute = parameter.GetCustomAttributeByReflection<DefaultValueAttribute>();
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        private static IActionDefaultsFacet Create(DefaultValueAttribute attribute, IFacetHolder holder) {
            return attribute == null ? null : new ActionDefaultsFacetAnnotation(attribute.Value, holder);
        }
    }
}