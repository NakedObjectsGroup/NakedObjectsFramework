// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Resolve;

namespace NakedObjects.Architecture.Facets.Hide {
    public class HiddenFacetImpl : HiddenFacetAbstract {
        public HiddenFacetImpl(WhenTo when, IFacetHolder holder)
            : base(when, holder) {}

        public override string HiddenReason(INakedObject target) {
            if (Value == WhenTo.Always) {
                return Resources.NakedObjects.AlwaysHidden;
            }
            if (Value == WhenTo.Never) {
                return null;
            }

            // remaining tests depend on target in question.
            if (target == null) {
                return null;
            }

            if (Value == WhenTo.UntilPersisted)
            {
                return target.ResolveState.IsTransient() ? Resources.NakedObjects.HiddenUntilPersisted : null;
            }
            if (Value == WhenTo.OncePersisted)
            {
                return target.ResolveState.IsPersistent() ? Resources.NakedObjects.HiddenOncePersisted : null;
            }
            return null;
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}