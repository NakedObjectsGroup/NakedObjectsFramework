// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Security;

namespace NakedObjects.Architecture.Facets.Disable {
    public abstract class DisableForSessionFacetAbstract : FacetAbstract, IDisableForSessionFacet {
        protected DisableForSessionFacetAbstract(IFacetHolder holder)
            : base(Type, holder) {}

        public static Type Type {
            get { return typeof (IDisableForSessionFacet); }
        }

        #region IDisableForSessionFacet Members

        public abstract string DisabledReason(ISession session, INakedObject target, INakedObjectPersistor persistor);

        #endregion
    }


    // Copyright (c) Naked Objects Group Ltd.
}