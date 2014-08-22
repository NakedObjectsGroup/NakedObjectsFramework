// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Objects.Ident.Icon;
using NakedObjects.Architecture.Facets.Objects.Ident.Plural;
using NakedObjects.Architecture.Persist;

namespace NakedObjects.Architecture.Facets.Objects.Ident.Title {
    /// <summary>
    ///     Mechanism for obtaining the title of an instance of a class, used to label the instance in the viewer
    ///     (usually alongside an icon representation)
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, typically corresponds to a method named <c>Title</c>
    /// </para>
    /// <seealso cref="IIconFacet" />
    /// <seealso cref="IPluralFacet" />
    public interface ITitleFacet : IFacet {
        string GetTitle(INakedObject nakedObject, INakedObjectManager manager);

        string GetTitleWithMask(string mask, INakedObject nakedObject, INakedObjectManager manager);
    }
}