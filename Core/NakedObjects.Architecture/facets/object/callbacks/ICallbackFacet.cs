// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Facets.Objects.Callbacks {
    /// <summary>
    ///     A <see cref="IFacet" /> that represents some type of lifecycle callback on the object (eg about to be persisted).
    /// </summary>
    public interface ICallbackFacet : IFacet {
        void Invoke(INakedObject nakedObject);
    }

    // Copyright (c) Naked Objects Group Ltd.
}