// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedFramework.Metamodel.SemanticsProvider;

public interface IValueSemanticsProvider<T> {
    bool IsImmutable { get; }

    /// <summary>
    ///     Whether the value has equal by content semantics
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         If so, then it must implement <c>Equals</c> and <c>GetHashCode</c>
    ///         consistently.  Examples  that do this are <see cref="string" /> for example
    ///     </para>
    /// </remarks>
    bool IsEqualByContent { get; }

    /// <summary>
    ///     The typical length of objects that can be parsed
    /// </summary>
    int TypicalLength { get; }

    T DefaultValue { get; }

    /// <summary>
    ///     Returns the provided object as an encoded string.
    /// </summary>
    /// <para>
    ///     Even if the class is self-encodeable, this method
    ///     is always called on a new instance of the object created via the
    ///     no-arg constructor.  That is, the object shouldn't encode itself,
    ///     it should encode the object provided to it.
    /// </para>
    /// <seealso cref="FromEncodedString" />
    string ToEncodedString(T toEncode);

    /// <summary>
    ///     Converts an encoded string to an instance of the object.
    /// </summary>
    /// <para>
    ///     Here the implementing class is acting as a factory for itself
    /// </para>
    /// <seealso cref="ToEncodedString" />
    T FromEncodedString(string encodedString);

    /// <summary>
    ///     Parses a string to an instance of the object
    /// </summary>
    /// <para>
    ///     Here the implementing class is acting as a factory for itself
    /// </para>
    /// <param name="entry"></param>
    object ParseTextEntry(string entry);

    /// <summary>
    ///     Parses a string to an instance of the object
    /// </summary>
    /// <para>
    ///     Here the implementing class is acting as a factory for itself
    /// </para>
    /// <param name="entry"></param>
    object ParseInvariant(string entry);

    /// <summary>
    ///     The opposite of ParseInvariant
    /// </summary>
    string InvariantString(T obj);

    /// <summary>
    ///     The title of the object
    /// </summary>
    string DisplayTitleOf(T obj);

    /// <summary>
    ///     The title of the object, with mask applied
    /// </summary>
    string TitleWithMaskOf(string mask, T obj);

    /// <summary>
    ///     A title for the object that is valid but which may be easier to
    ///     edit than the title provided by a <c>NakedObjects.Architecture.Facets.Objects.Ident.Title.ITitleFacet</c>
    /// </summary>
    /// <para>
    ///     The idea here is that the viewer can display a parseable title
    ///     for an existing object when, for example, the user initially
    ///     clicks in the field.  So, a date might be rendered via a
    ///     ITitleFacet as May 2, 2007, but its parseable
    ///     form might be 20070502
    /// </para>
    string EditableTitleOf(T existing);
}