// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Capabilities {
    public interface IValueSemanticsProvider<T> {
        bool IsImmutable { get; }

        /// <summary>
        ///     Whether the value has <see cref="EqualByContentAttribute" /> semantics
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         If so, then it must implement <c>Equals</c> and <c>GetHashCode</c>
        ///         consistently.  Examples  that do this are <see cref="string" /> for example
        ///     </para>
        /// </remarks>
        bool IsEqualByContent { get; }

        /// <summary>
        ///     The <see cref="IParser{T}" />, if any
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         If not <c>null</c>, implies that the value is <see cref="ParseableAttribute" />
        ///     </para>
        /// </remarks>
        IParser<T> Parser { get; }

        /// <summary>
        ///     The <see cref="IEncoderDecoder{T}" />, if any
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         If not <c>null</c>, implies that the value is <see cref="EncodeableAttribute" />
        ///     </para>
        /// </remarks>
        IEncoderDecoder<T> EncoderDecoder { get; }

        /// <summary>
        ///     The <see cref="IDefaultsProvider{T}" />, if any
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         If not <c>null</c>, implies that the value has (or may have) a default
        ///     </para>
        /// </remarks>
        IDefaultsProvider<T> DefaultsProvider { get; }

        /// <summary>
        ///     The <see cref="IFromStream" />, if any
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         If not <c>null</c>, implies that the value is creatable from a stream (not specifiable by attribute).
        ///     </para>
        /// </remarks>
        IFromStream FromStream { get; }
    }
}