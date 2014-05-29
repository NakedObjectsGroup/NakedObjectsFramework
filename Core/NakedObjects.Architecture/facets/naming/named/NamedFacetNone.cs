// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Facets.Naming.Named {
    /// <summary>
    ///     Has a name of <c>null</c>
    /// </summary>
    // TODO: should this instead be the empty string?  
    public class NamedFacetNone : NamedFacetAbstract {
        public NamedFacetNone(IFacetHolder holder)
            : base(null, holder) {}

        public override bool IsNoOp {
            get { return true; }
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}