// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Ordering.MemberOrder;

namespace NakedObjects.Reflector.DotNet.Facets.Ordering.MemberOrder {
    public class MemberOrderFacetAnnotation : MemberOrderFacetAbstract {
        public MemberOrderFacetAnnotation(string name, string sequence, IFacetHolder holder)
            : base(name, sequence, holder) {}
    }

    // Copyright (c) Naked Objects Group Ltd.
}