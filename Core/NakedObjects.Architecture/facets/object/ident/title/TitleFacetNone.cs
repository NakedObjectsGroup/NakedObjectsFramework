// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;

namespace NakedObjects.Architecture.Facets.Objects.Ident.Title {
    public class TitleFacetNone : TitleFacetAbstract {
        public TitleFacetNone(IFacetHolder holder)
            : base(holder) {}

        public override bool IsNoOp {
            get { return true; }
        }

        public override string GetTitle(INakedObject nakedObject, INakedObjectManager manager) {
            return null;
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}