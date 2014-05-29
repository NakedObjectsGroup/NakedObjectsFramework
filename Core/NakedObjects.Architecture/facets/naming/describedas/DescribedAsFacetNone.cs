// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Facets.Naming.DescribedAs {
    /// <summary>
    ///     Has a description of the empty string
    /// </summary>
    public class DescribedAsFacetNone : DescribedAsFacetAbstract {
        public DescribedAsFacetNone(IFacetHolder holder)
            : base("", holder) {}

        public override bool IsNoOp {
            get { return true; }
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}