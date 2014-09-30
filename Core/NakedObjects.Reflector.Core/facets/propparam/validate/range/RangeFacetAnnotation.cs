// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Propparam.Validate.Range;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.Range {
    public class RangeFacetAnnotation : RangeFacetAbstract {
        public RangeFacetAnnotation(object min, object max, bool isDateRange, IFacetHolder holder)
            : base(min, max, holder) {
            IsDateRange = isDateRange;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}