// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Objects.Ident.Plural;
using NakedObjects.Architecture.Facets.Objects.Ident.Title;

namespace NakedObjects.Architecture.Facets.Objects.Ident.Icon {
    /// <summary>
    ///     Mechanism for obtaining the name of the icon.
    /// </summary>
    /// <para>
    ///     Typically an icon based on the class name is used for every instance of a class (for example, by placing an appropriately
    ///     named image file into a certain directory). This facet allows the icon to be changed for the class or on an
    ///     instance-by-instance basis. For example, the icon might be adapted with an overlay to represent its state
    ///     through some well-defined lifecycle (eg pending approval, approved, rejected). Alternatively a
    ///     <see cref="BoundedAttribute" /> annotated class might have completely different icons for its instances (eg Visa,
    ///     Mastercard, Amex).
    /// </para>
    /// <para>
    ///     In the standard Naked Objects Programming Model, typically corresponds to a method named <c>iconName</c>.
    /// </para>
    /// <seealso cref="ITitleFacet" />
    /// <seealso cref="IPluralFacet" />
    public interface IIconFacet : IFacet {
        /// <summary>
        ///     The name of the icon for <i>this instance</i> of a class.
        /// </summary>
        /// <para>
        ///     In the standard Naked Objects Programming Model, this typically corresponds to a method named <c>iconName</c>.
        /// </para>
        string GetIconName(INakedObject nakedObject);

        /// <summary>
        ///     The name of the icon for <i>this class</i> of object.
        /// </summary>
        /// <para>
        ///     In the standard Naked Objects Programming Model, this typically corresponds to the <see cref="MaskAttribute" />.
        /// </para>
        string GetIconName();
    }
}