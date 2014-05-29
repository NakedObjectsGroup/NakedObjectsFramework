// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Resolve;

namespace NakedObjects.Architecture.Facets.Objects.Immutable {
    public class ImmutableFacetImpl : ImmutableFacetAbstract {
        public ImmutableFacetImpl(When when, IFacetHolder holder)
            : base(when, holder) {}

        public override string DisabledReason(INakedObject target) {
            if (Value == When.Always) {
                return string.Format(Resources.NakedObjects.ImmutableMessage, target.Specification.SingularName);
            }
            if (Value == When.Never) {
                return null;
            }

            // remaining tests depend on target in question.
            if (target == null) {
                return null;
            }
            if (Value == When.UntilPersisted) {
                return target.ResolveState.IsTransient() ? string.Format(Resources.NakedObjects.ImmutableUntilPersistedMessage, target.Specification.SingularName) : null;
            }
            if (Value == When.OncePersisted) {
                return target.ResolveState.IsPersistent() ? string.Format(Resources.NakedObjects.ImmutableOncePersistedMessage, target.Specification.SingularName) : null;
            }
            return null;
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}