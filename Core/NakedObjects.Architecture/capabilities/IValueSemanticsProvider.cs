// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

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