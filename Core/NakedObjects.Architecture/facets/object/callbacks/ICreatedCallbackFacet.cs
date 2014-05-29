// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Facets.Objects.Callbacks {
    /// <summary>
    ///     Represents the mechanism to inform the object that it has just been created.
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, this is represented
    ///     by a <c>Created</c> method.  The framework calls this once the object
    ///     has been created via <c>NewTransientInstance</c> or
    ///     <c>NewInstance</c>.  The method is <i>not</i> called when the object
    ///     is subsequently resolved having been persisted; for that see
    ///     <see cref="ILoadingCallbackFacet" /> and <see cref="ILoadedCallbackFacet" />
    /// </para>
    /// <seealso cref="ILoadingCallbackFacet" />
    /// <seealso cref="ILoadedCallbackFacet" />
    public interface ICreatedCallbackFacet : ICallbackFacet {}

    // Copyright (c) Naked Objects Group Ltd.
}