// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Facets.Objects.TypicalLength {
    public class TypicalLengthFacetZero : TypicalLengthFacetAbstract {
        public TypicalLengthFacetZero(IFacetHolder holder)
            : base(0, holder) {}

        public override bool IsNoOp {
            get { return true; }
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}