// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Disable;

namespace NakedObjects.Reflector.DotNet.Facets.Disable {
    public class DisabledFacetAnnotation : DisabledFacetImpl {
        public DisabledFacetAnnotation(When value, IFacetHolder holder)
            : base(value, holder) {}
    }


    // Copyright (c) Naked Objects Group Ltd.
}