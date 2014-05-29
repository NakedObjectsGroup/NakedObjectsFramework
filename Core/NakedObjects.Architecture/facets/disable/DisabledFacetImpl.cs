// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Resolve;

namespace NakedObjects.Architecture.Facets.Disable {
    public class DisabledFacetImpl : DisabledFacetAbstract {
        public DisabledFacetImpl(When when, IFacetHolder holder)
            : base(when, holder) {}

        public override string DisabledReason(INakedObject target) {
            if (Value == When.Always) {
                return Resources.NakedObjects.AlwaysDisabled;
            }
            if (Value == When.Never) {
                return null;
            }

            // remaining tests depend upon the actual target in question
            if (target == null) {
                return null;
            }
            if (Value == When.UntilPersisted) {
                return target.ResolveState.IsTransient() ? Resources.NakedObjects.DisabledUntilPersisted : null;
            }
            if (Value == When.OncePersisted) {
                return target.ResolveState.IsPersistent() ? Resources.NakedObjects.DisabledOncePersisted : null;
            }
            return null;
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}