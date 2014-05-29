// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Facets.Objects.Callbacks {
    public abstract class CallbackWithExceptionFacetAbstract : FacetAbstract, ICallbackWithExceptionFacet {
        protected CallbackWithExceptionFacetAbstract(Type facetType, IFacetHolder holder)
            : base(facetType, holder) {}

        #region ICallbackWithExceptionFacet Members

        public abstract string Invoke(INakedObject nakedObject, Exception exception);

        #endregion
    }
}