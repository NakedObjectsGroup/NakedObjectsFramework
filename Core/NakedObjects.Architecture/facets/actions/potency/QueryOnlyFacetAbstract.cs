// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Facets.Actions.Potency {
    public abstract class QueryOnlyFacetAbstract : MarkerFacetAbstract, IQueryOnlyFacet {
        protected QueryOnlyFacetAbstract(IFacetHolder holder)
            : base(Type, holder) {}

        public static Type Type {
            get { return typeof (IQueryOnlyFacet); }
        }
    }
}