// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Objects.Ident.Title;
using NakedObjects.Architecture.Persist;
using NakedObjects.Capabilities;

namespace NakedObjects.Architecture.Facets.Objects.Parseable {
    /// <summary>
    ///     Indicates that this class can parse an entry string
    /// </summary>
    public interface IParseableFacet : IMultipleValueFacet {
        bool IsValid { get; }

        /// <summary>
        ///     Parses a text entry made by a user and sets the domain object's value.
        /// </summary>
        /// <para>
        ///     Equivalent to <see cref="IParser{T}.ParseTextEntry" />, though may
        ///     be implemented through some other mechanism.
        /// </para>
        /// <exception cref="InvalidEntryException" />
        INakedObject ParseTextEntry(string text, INakedObjectManager manager);

        /// <summary>
        ///     Parses an invariant value and sets the domain objects value
        /// </summary>
        /// <para>
        ///     Equivalent to <see cref="IParser{T}.ParseTextEntry" />, though may
        ///     be implemented through some other mechanism.
        /// </para>
        /// <exception cref="InvalidEntryException" />
        INakedObject ParseInvariant(string text, INakedObjectManager manager);


        /// <summary>
        ///     A title for the object that is valid but which may be easier to
        ///     edit than the title provided by a <see cref="ITitleFacet" />
        /// </summary>
        /// <para>
        ///     The idea here is that the viewer can display a parseable title
        ///     for an existing object when, for example, the user initially
        ///     clicks in the field.  So, a date might be rendered via a
        ///     <see cref="ITitleFacet" /> as <b>May 2, 2007</b>, but its parseable
        ///     form might be <b>20070502</b>.
        /// </para>
        string ParseableTitle(INakedObject nakedObject);


        string InvariantString(INakedObject nakedObject);
    }
}