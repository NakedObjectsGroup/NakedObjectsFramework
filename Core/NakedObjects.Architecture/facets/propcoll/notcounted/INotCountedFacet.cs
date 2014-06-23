// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Facets.Propcoll.NotCounted {
    /// <summary>
    ///     Indicates a collection should not be counted in the summary view.
    ///     This is only used by the custom 'SdmNotCountedAttribute'
    /// </summary>
    public interface INotCountedFacet : IMarkerFacet {}
}