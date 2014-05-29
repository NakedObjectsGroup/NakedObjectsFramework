// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Facets.Propcoll.NotPersisted {
    /// <summary>
    ///     Indicates that a property or a collection shouldn't be persisted
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to annotating the property
    ///     or collection with the <see cref="NotPersistedAttribute" /> annotation
    /// </para>
    // TODO: need to reconcile with  IDerivedFacet that has very similar semantics for properties
    public interface INotPersistedFacet : IMarkerFacet {}
}