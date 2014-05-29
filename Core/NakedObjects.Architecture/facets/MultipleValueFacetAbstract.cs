// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Facets {
    public abstract class MultipleValueFacetAbstract : FacetAbstract, IMultipleValueFacet {
        protected MultipleValueFacetAbstract(Type facetType, IFacetHolder holder)
            : base(facetType, holder) {}
    }
}