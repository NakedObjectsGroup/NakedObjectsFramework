// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Propparam.Validate.MaxLength;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.MaxLength {
    public class MaxLengthFacetAnnotation : MaxLengthFacetAbstract {
        public MaxLengthFacetAnnotation(int value, IFacetHolder holder)
            : base(value, holder) {}
    }

    // Copyright (c) Naked Objects Group Ltd.
}