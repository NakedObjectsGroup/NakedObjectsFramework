// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Facets.Propcoll.NotCounted {
    /// <summary>
    ///     Indicates a collection should not be counted in the summary view.
    ///     There is currently no attribute or programming convention in the
    ///     standard ProgrammingModel that makes use of this, but developers
    ///     may add their own, and this is known to be in use in one site at least.
    /// </summary>
    public interface INotCountedFacet : IMarkerFacet {}
}