// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Security;

namespace NakedObjects.Architecture.Facets.Hide {
    public class HideForSessionFacetNone : HideForSessionFacetAbstract {
        public HideForSessionFacetNone(IFacetHolder holder)
            : base(holder) {}

        public override bool IsNoOp {
            get { return true; }
        }

        /// <summary>
        ///     Always returns <c>null</c>
        /// </summary>
        public override string HiddenReason(ISession session, INakedObject target, INakedObjectPersistor persistor) {
            return null;
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}