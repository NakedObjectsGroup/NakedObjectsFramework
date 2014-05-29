// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets.Objects.Ident.Icon;
using NakedObjects.Architecture.Facets.Objects.Ident.Title;

namespace NakedObjects.Architecture.Facets.Objects.Ident.Plural {
    /// <summary>
    ///     Mechanism for obtaining the plural title of an instance of a class, used to label a collection of
    ///     a certain class
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, typically corresponds to a method named <c>PluralName</c>.
    ///     If no plural name is provided, then the framework will attempt to guess the plural name (by adding an
    ///     <i>s</i> or <i>ies</i> suffix).
    /// </para>
    /// <seealso cref="IIconFacet" />
    /// <seealso cref="ITitleFacet" />
    public interface IPluralFacet : ISingleStringValueFacet {}
}