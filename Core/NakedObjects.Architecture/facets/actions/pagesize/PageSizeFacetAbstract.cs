// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Facets.Actions.PageSize {
    public abstract class PageSizeFacetAbstract : SingleIntValueFacetAbstract, IPageSizeFacet {
        protected PageSizeFacetAbstract(int pageSize, IFacetHolder holder)
            : base(Type, holder, pageSize) {}

        public static Type Type {
            get { return typeof (IPageSizeFacet); }
        }
    }
}