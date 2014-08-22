// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Encodeable;
using NakedObjects.Architecture.Persist;
using NakedObjects.Capabilities;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Encodeable {
    public abstract class EncodeableFacetAbstract<T> : FacetAbstract, IEncodeableFacet {
        // to delegate to
        private readonly EncodeableFacetUsingEncoderDecoder<T> encodeableFacetUsingEncoderDecoder;
        private readonly Type encoderDecoderClass;

        protected EncodeableFacetAbstract(string candidateEncoderDecoderName,
                                          Type candidateEncoderDecoderClass,
                                          IFacetHolder holder)
            : base(typeof (IEncodeableFacet), holder) {
            encoderDecoderClass = EncoderDecoderUtils.EncoderDecoderOrNull<T>(candidateEncoderDecoderClass, candidateEncoderDecoderName);
            encodeableFacetUsingEncoderDecoder = IsValid ? new EncodeableFacetUsingEncoderDecoder<T>((IEncoderDecoder<T>) TypeUtils.NewInstance(encoderDecoderClass), holder)
                                                     : null;
        }

        #region IEncodeableFacet Members

        public INakedObject FromEncodedString(string encodedData, INakedObjectManager manager) {
            return encodeableFacetUsingEncoderDecoder.FromEncodedString(encodedData, manager);
        }

        public string ToEncodedString(INakedObject nakedObject) {
            return encodeableFacetUsingEncoderDecoder.ToEncodedString(nakedObject);
        }

        /// <summary>
        ///     Discover whether either of the candidate encoder/decoder name or class is valid.
        /// </summary>
        public bool IsValid {
            get { return encoderDecoderClass != null; }
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
    }
}