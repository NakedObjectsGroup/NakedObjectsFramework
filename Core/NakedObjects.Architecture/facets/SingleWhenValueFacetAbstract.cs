// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Facets {
    public abstract class SingleWhenValueFacetAbstract : FacetAbstract, ISingleWhenValueFacet {
        private readonly When when;

        protected SingleWhenValueFacetAbstract(Type facetType, IFacetHolder holder, When when)
            : base(facetType, holder) {
            this.when = when;
        }

        #region ISingleWhenValueFacet Members

        public virtual When Value {
            get { return when; }
        }

        #endregion

        protected override string ToStringValues() {
            return "when=" + when;
        }
    }
}