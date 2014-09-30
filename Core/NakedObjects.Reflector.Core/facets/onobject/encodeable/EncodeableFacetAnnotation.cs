// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Encodeable {
    public class EncodeableFacetAnnotation<T> : EncodeableFacetAbstract<T> {
        public EncodeableFacetAnnotation(Type annotatedClass, IFacetHolder holder)
            : this(EncoderDecoderName(annotatedClass), EncoderDecoderClass(annotatedClass), holder) {}

        private EncodeableFacetAnnotation(string candidateEncoderDecoderName, Type candidateEncoderDecoderClass, IFacetHolder holder)
            : base(candidateEncoderDecoderName, candidateEncoderDecoderClass, holder) {}

        private static string EncoderDecoderName(Type annotatedClass) {
            var annotation = annotatedClass.GetCustomAttributeByReflection<EncodeableAttribute>();
            string encoderDecoderName = annotation.EncoderDecoderName;
            return !string.IsNullOrEmpty(encoderDecoderName) ? encoderDecoderName : null;
        }

        private static Type EncoderDecoderClass(Type annotatedClass) {
            var annotation = annotatedClass.GetCustomAttributeByReflection<EncodeableAttribute>();
            return annotation.EncoderDecoderClass;
        }
    }
}