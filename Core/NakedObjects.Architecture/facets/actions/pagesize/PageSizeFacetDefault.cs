// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Facets.Actions.PageSize {
    public class PageSizeFacetDefault : PageSizeFacetAbstract {
        private const int defaultPageSize = 20;

        public PageSizeFacetDefault(IFacetHolder holder)
            : base(defaultPageSize, holder) {}
    }


    // Copyright (c) Naked Objects Group Ltd.
}