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
    ///     The typical length of objects that can be parsed
    /// </summary>
    int TypicalLength { get; }

    T DefaultValue { get; }

    /// <summary>
    ///     Parses a string to an instance of the object
    /// </summary>
    /// <para>
    ///     Here the implementing class is acting as a factory for itself
    /// </para>
    /// <param name="entry"></param>
    object ParseTextEntry(string entry);

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