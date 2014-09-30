// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Defaults {
    public class DefaultedFacetAnnotation<T> : DefaultedFacetAbstract<T> {
        public DefaultedFacetAnnotation(Type annotatedClass, IFacetHolder holder)
            : this(ProviderName(annotatedClass), ProviderClass(annotatedClass), holder) {}

        private DefaultedFacetAnnotation(string candidateProviderName, Type candidateProviderClass, IFacetHolder holder)
            : base(candidateProviderName, candidateProviderClass, holder) {}

        private static string ProviderName(Type annotatedClass) {
            var annotation = annotatedClass.GetCustomAttributeByReflection<DefaultedAttribute>();
            string providerName = annotation.DefaultsProviderName;
            return !string.IsNullOrEmpty(providerName) ? providerName : null;
        }

        private static Type ProviderClass(Type annotatedClass) {
            var annotation = annotatedClass.GetCustomAttributeByReflection<DefaultedAttribute>();
            return annotation.DefaultsProviderClass;
        }
    }
}