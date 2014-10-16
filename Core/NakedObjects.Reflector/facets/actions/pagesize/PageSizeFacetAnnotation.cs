// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actions.PageSize;

namespace NakedObjects.Reflector.DotNet.Facets.Actions.PageSize {
    public class PageSizeFacetAnnotation : PageSizeFacetAbstract {
        public PageSizeFacetAnnotation(int pageSize, ISpecification holder)
            : base(pageSize, holder) {}
    }

    // Copyright (c) Naked Objects Group Ltd.
}