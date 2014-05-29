// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Facets.Objects.Bounded {
    public class BoundedFacetImpl : BoundedFacetAbstract {
        public BoundedFacetImpl(IFacetHolder holder)
            : base(holder) {}

        public override string DisabledReason(INakedObject inObject) {
            return Resources.NakedObjects.Bounded;
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}