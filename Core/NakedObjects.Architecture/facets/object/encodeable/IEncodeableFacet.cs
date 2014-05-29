// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Capabilities;

namespace NakedObjects.Architecture.Facets.Objects.Encodeable {
    /// <summary>
    ///     Indicates that this class can be encoded/decoded as a string
    /// </summary>
    public interface IEncodeableFacet : IMultipleValueFacet {
        bool IsValid { get; }

        /// <summary>
        ///     Equivalent to <see cref="IEncoderDecoder{T}.FromEncodedString" /> though may be
        ///     implemented through some other equivalent mechanism.
        /// </summary>
        INakedObject FromEncodedString(string encodedData);

        /// <summary>
        ///     Equivalent to <see cref="IEncoderDecoder{T}.ToEncodedString" />, though may be
        ///     implemented through some other equivalent mechanism.
        /// </summary>
        string ToEncodedString(INakedObject nakedObject);
    }
}