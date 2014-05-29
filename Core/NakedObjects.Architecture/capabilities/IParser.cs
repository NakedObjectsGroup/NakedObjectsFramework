// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Capabilities {
    /// <summary>
    ///     Provides a mechanism for parsing and rendering string representations of objects
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Specifically, this interface embodies three related capabilties
    ///     </para>
    ///     <list type="bullet">
    ///         <item>to parse a string representation and convert to an object</item>
    ///         <item>to provide a string representation of the object, for use as its title</item>
    ///         <item>to indicate the typical length of such a string representation</item>
    ///     </list>
    ///     <para>
    ///         For custom-written (as opposed to third-party) value types, the ability for the <see cref="IParser{T}" />
    ///         to provide a title responsibilities overlap with other conventions for domain objects.
    ///         Specifically, normally we write a <c>Title()</c> method to return a title.
    ///         In such cases a typical implementation of <see cref="IParser{T}" /> would just delegate to the value type
    ///         itself to obtain the title (ie invoking the <c>Title()</c> method directly rather than
    ///         having the framework do this)
    ///     </para>
    ///     <para>
    ///         Similarly, the ability to return a typical length also overlaps with the <see cref="TypicalLengthAttribute" /> annotation;
    ///         which is why <see cref="TypicalLengthAttribute" /> cannot be applied to types, only to properties and parameters
    ///     </para>
    ///     <para>
    ///         For third-party value types, there is no ability to write <c>Title()</c> methods or annotated with
    ///         <see
    ///             cref="TypicalLengthAttribute" />
    ///         ;
    ///         so this is the main reason that this interface has to deal with titles and lengths
    ///     </para>
    ///     <para>
    ///         This interface is used in two complementary ways:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>
    ///             As one option, it allows objects to
    ///             take control of their own parsing, by implementing directly.
    ///             However, the instance is used as a factory for itself.  The framework will instantiate
    ///             an instance, invoke the appropriate method method, and use the returned object.
    ///             The instantiated instance itself will be discarded.
    ///         </item>
    ///         <item>
    ///             Alternatively, an implementor of this interface can be
    ///             nominated in the <see cref="ParseableAttribute" /> annotation,
    ///             allowing a class that needs to be parseable to indicate how it can be parsed
    ///         </item>
    ///     </list>
    ///     <para>
    ///         Whatever the class that implements this interface, it must also expose a
    ///         <c>public</c> no-arg constructor so that the framework can instantiate the object.
    ///     </para>
    /// </remarks>
    /// <seealso cref="IEncoderDecoder{T}" />
    public interface IParser<T> {
        /// <summary>
        ///     The typical length of objects that can be parsed
        /// </summary>
        int TypicalLength { get; }

        /// <summary>
        ///     Parses a string to an instance of the object
        /// </summary>
        /// <para>
        ///     Here the implementing class is acting as a factory for itself
        /// </para>
        /// <param name="context">The context in which the text is being parsed.  For example +3 might mean add 3 to the current number</param>
        /// <param name="entry"></param>
        object ParseTextEntry(T context, string entry);

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
}