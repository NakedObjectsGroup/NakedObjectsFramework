// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Facets.Collections.Modify {
    /// <summary>
    ///     Clear all objects from a collection
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to
    ///     either invoking the <c>ClearXxx</c> support method, or just
    ///     invoking <c>Clear</c> on the collection returned by the accessor method
    /// </para>
    public interface ICollectionResetFacet : IFacet {
        void Reset(INakedObject inObject);
    }
}