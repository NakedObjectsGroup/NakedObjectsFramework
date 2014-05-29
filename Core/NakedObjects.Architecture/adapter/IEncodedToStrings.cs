// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Adapter {
    /// <summary>
    ///     Indicates that the implementing class should be able to be encoded to an array of strings
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The implementing class should also have a constructor that accepts an array of strings and
    ///         initialises from them
    ///     </para>
    /// </remarks>
    /// <seealso cref="StringEncoderHelper" />
    /// <seealso cref="StringDecoderHelper" />
    public interface IEncodedToStrings {
        string[] ToEncodedStrings();
        string[] ToShortEncodedStrings();
    }
}