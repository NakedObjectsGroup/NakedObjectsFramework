// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Facets.Objects.Callbacks {
    /// <summary>
    ///     Represents the mechanism to inform the object that it is about to be persisted to the object store for the first time
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, this is represented
    ///     by a <c>Persisting</c> method. Called only if the object is known to be in a valid state.
    /// </para>
    /// <seealso cref="IPersistedCallbackFacet" />
    public interface IPersistingCallbackFacet : ICallbackFacet {}

    // Copyright (c) Naked Objects Group Ltd.
}