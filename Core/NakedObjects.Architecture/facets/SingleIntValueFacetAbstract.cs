// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Facets {
    public abstract class SingleIntValueFacetAbstract : FacetAbstract, ISingleIntValueFacet {
        private readonly int valueInt;

        protected SingleIntValueFacetAbstract(Type facetType, IFacetHolder holder, int valueInt)
            : base(facetType, holder) {
            this.valueInt = valueInt;
        }

        #region ISingleIntValueFacet Members

        public virtual int Value {
            get { return valueInt; }
        }

        #endregion
    }
}