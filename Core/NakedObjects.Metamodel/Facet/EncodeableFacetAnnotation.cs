// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Spec;
using NakedObjects.Util;

namespace NakedObjects.Metamodel.Facet {
    public class EncodeableFacetAnnotation<T> : EncodeableFacetAbstract<T> {
        public EncodeableFacetAnnotation(Type annotatedClass, ISpecification holder)
            : this(EncoderDecoderName(annotatedClass), EncoderDecoderClass(annotatedClass), holder) {}

        private EncodeableFacetAnnotation(string candidateEncoderDecoderName, Type candidateEncoderDecoderClass, ISpecification holder)
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