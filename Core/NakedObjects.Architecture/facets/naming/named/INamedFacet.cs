// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Facets.Naming.Named {
    /// <summary>
    ///     The name of a class, a property, collection, an action  or a parameter
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to
    ///     annotating the member with <see cref="NamedAttribute" />
    /// </para>
    public interface INamedFacet : ISingleStringValueFacet {}
}