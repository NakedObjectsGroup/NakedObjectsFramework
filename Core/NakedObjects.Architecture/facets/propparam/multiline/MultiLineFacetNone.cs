// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Facets.Propparam.MultiLine {
    public class MultiLineFacetNone : MultiLineFacetAbstract {
        public MultiLineFacetNone(IFacetHolder holder)
            : base(1, 0, holder) {}

        public override bool IsNoOp {
            get { return true; }
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}