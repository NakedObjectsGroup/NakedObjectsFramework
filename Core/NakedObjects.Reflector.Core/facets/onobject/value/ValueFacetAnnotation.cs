// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Value {
    public class ValueFacetAnnotation<T> : ValueFacetAbstract<T> {
        public ValueFacetAnnotation(Type annotatedClass, IFacetHolder holder)
            : this(SemanticsProviderName(annotatedClass), SemanticsProviderClass(annotatedClass), holder) {}

        private ValueFacetAnnotation(string candidateSemanticsProviderName,
                                     Type candidateSemanticsProviderClass,
                                     IFacetHolder holder)
            : base(ValueSemanticsProviderUtils.ValueSemanticsProviderOrNull<T>(candidateSemanticsProviderClass,
                                                                               candidateSemanticsProviderName),
                   true,
                   holder) {}

        /// <summary>
        ///     Always valid, even if the specified semanticsProviderName might have been wrong.
        /// </summary>
        public override bool IsValid {
            get { return true; }
        }

        private static string SemanticsProviderName(Type annotatedClass) {
            string semanticsProviderName = annotatedClass.GetCustomAttributeByReflection<ValueAttribute>().SemanticsProviderName;
            return string.IsNullOrEmpty(semanticsProviderName) ? null : semanticsProviderName;
        }

        private static Type SemanticsProviderClass(Type annotatedClass) {
            return annotatedClass.GetCustomAttributeByReflection<ValueAttribute>().SemanticsProviderClass;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}