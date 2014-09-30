// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Encodeable;
using NakedObjects.Architecture.Persist;
using NakedObjects.Capabilities;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Encodeable {
    public class EncodeableFacetUsingEncoderDecoder<T> : FacetAbstract, IEncodeableFacet {
        public static string ENCODED_NULL = "NULL";
        private readonly IEncoderDecoder<T> encoderDecoder;

        public EncodeableFacetUsingEncoderDecoder(IEncoderDecoder<T> encoderDecoder, IFacetHolder holder)
            : base(typeof (IEncodeableFacet), holder) {
            this.encoderDecoder = encoderDecoder;
        }

        #region IEncodeableFacet Members

        public INakedObject FromEncodedString(string encodedData, INakedObjectManager manager) {
            //Assert.assertNotNull(encodedData);
            if (ENCODED_NULL.Equals(encodedData)) {
                return null;
            }
            return manager.CreateAdapter(encoderDecoder.FromEncodedString(encodedData), null, null);
        }

        public string ToEncodedString(INakedObject nakedObject) {
            return nakedObject == null ? ENCODED_NULL : encoderDecoder.ToEncodedString(nakedObject.GetDomainObject<T>());
        }

        public bool IsValid {
            get { return encoderDecoder != null; }
        }

        #endregion

        // TODO: is this safe? really?

        protected override string ToStringValues() {
            return encoderDecoder.ToString();
        }
    }
}