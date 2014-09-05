// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Immutable;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Immutable {
    public class ImmutableFacetAnnotation : ImmutableFacetImpl {
        public ImmutableFacetAnnotation(WhenTo value, IFacetHolder holder)
            : base(value, holder) {}
    }

    // Copyright (c) Naked Objects Group Ltd.
}