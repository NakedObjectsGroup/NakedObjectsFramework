// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Facets.Objects.Callbacks {
    /// <summary>
    ///     Adapter superclass for <see cref="IFacet" />s for <see cref="ICallbackFacet" />
    /// </summary>
    public abstract class CallbackFacetAbstract : FacetAbstract, ICallbackFacet {
        protected CallbackFacetAbstract(Type facetType, IFacetHolder holder)
            : base(facetType, holder) {}

        #region ICallbackFacet Members

        public abstract void Invoke(INakedObject nakedObject);

        #endregion
    }


    // Copyright (c) Naked Objects Group Ltd.
}