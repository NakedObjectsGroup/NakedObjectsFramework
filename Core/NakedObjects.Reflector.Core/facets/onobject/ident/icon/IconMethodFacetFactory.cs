// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Ident.Icon;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Ident.Icon {
    public class IconMethodFacetFactory : MethodPrefixBasedFacetFactoryAbstract {
        private static readonly string[] prefixes;

        static IconMethodFacetFactory() {
            prefixes = new[] {PrefixesAndRecognisedMethods.IconNameMethod};
        }

        public IconMethodFacetFactory(INakedObjectReflector reflector)
            : base(reflector, NakedObjectFeatureType.ObjectsOnly) { }

        public override string[] Prefixes {
            get { return prefixes; }
        }

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder facetHolder) {
            MethodInfo method = FindMethod(type, MethodType.Object, PrefixesAndRecognisedMethods.IconNameMethod, typeof (string), Type.EmptyTypes);
            var attribute = type.GetCustomAttributeByReflection<IconNameAttribute>();
            if (method != null) {
                RemoveMethod(methodRemover, method);
                return FacetUtils.AddFacet(new IconFacetViaMethod(method, facetHolder, attribute == null ? null : attribute.Value));
            }

            return FacetUtils.AddFacet(Create(attribute, facetHolder));
        }


        private static IIconFacet Create(IconNameAttribute attribute, IFacetHolder holder) {
            return attribute != null ? new IconFacetAnnotation(attribute.Value, holder) : null;
        }
    }
}