// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets.Objects.Immutable;

namespace NakedObjects.Architecture.Facets.Objects.Value {
    public class ImmutableFacetViaValueSemantics : ImmutableFacetImpl {
        public ImmutableFacetViaValueSemantics(IFacetHolder holder)
            : base(When.Always, holder) {}
    }


    // Copyright (c) Naked Objects Group Ltd.
}