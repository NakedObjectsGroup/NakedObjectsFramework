// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Facets {
    public abstract class SingleStringValueFacetAbstract : FacetAbstract, ISingleStringValueFacet {
        private readonly string valueString;

        protected SingleStringValueFacetAbstract(Type facetType, IFacetHolder holder, string valueString)
            : base(facetType, holder) {
            this.valueString = valueString;
        }

        #region ISingleStringValueFacet Members

        public virtual string Value {
            get { return valueString; }
        }

        #endregion

        protected override string ToStringValues() {
            return valueString == null ? "null" : "\"" + valueString + "\"";
        }
    }
}