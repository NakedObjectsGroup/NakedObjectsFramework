// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Capabilities;
using NakedObjects.Util;
using NakedObjects.Metamodel.Utils;

namespace NakedObjects.Metamodel.Facet {
    public class EncodeableFacet<T> : FacetAbstract, IEncodeableFacet {
        private readonly EncodeableFacetUsingEncoderDecoder<T> encodeableFacetUsingEncoderDecoder;
        private readonly Type encoderDecoderClass;

        private EncodeableFacet(string candidateEncoderDecoderName,
                                          Type candidateEncoderDecoderClass,
                                          ISpecification holder)
            : base(typeof (IEncodeableFacet), holder) {
            encoderDecoderClass = EncoderDecoderUtils.EncoderDecoderOrNull<T>(candidateEncoderDecoderClass, candidateEncoderDecoderName);
            encodeableFacetUsingEncoderDecoder = IsValid ? new EncodeableFacetUsingEncoderDecoder<T>((IEncoderDecoder<T>) TypeUtils.NewInstance(encoderDecoderClass), holder)
                : null;
        }

        public EncodeableFacet(Type annotatedClass, ISpecification holder)
            : this(EncoderDecoderName(annotatedClass), EncoderDecoderClass(annotatedClass), holder) {}

        #region IEncodeableFacet Members

        /// <summary>
        ///     Discover whether either of the candidate encoder/decoder name or class is valid.
        /// </summary>
        public bool IsValid {
            get { return encoderDecoderClass != null; }
        }

        public INakedObject FromEncodedString(string encodedData, INakedObjectManager manager) {
            return encodeableFacetUsingEncoderDecoder.FromEncodedString(encodedData, manager);
        }

        public string ToEncodedString(INakedObject nakedObject) {
            return encodeableFacetUsingEncoderDecoder.ToEncodedString(nakedObject);
        }

        #endregion

        /// <summary>
        ///     Guaranteed to implement the <see cref="IEncoderDecoder{T}" /> class, thanks to generics in the applib.
        /// </summary>
        public Type GetEncoderDecoderClass() {
            return encoderDecoderClass;
        }

        protected override string ToStringValues() {
            return encoderDecoderClass.FullName;
        }

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