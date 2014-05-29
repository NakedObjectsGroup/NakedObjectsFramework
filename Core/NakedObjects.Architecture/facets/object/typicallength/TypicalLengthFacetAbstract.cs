// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Facets.Objects.TypicalLength {
    public abstract class TypicalLengthFacetAbstract : SingleIntValueFacetAbstract, ITypicalLengthFacet {
        protected TypicalLengthFacetAbstract(int intValue, IFacetHolder holder)
            : base(Type, holder, intValue) {}

        public static Type Type {
            get { return typeof (ITypicalLengthFacet); }
        }

        protected override string ToStringValues() {
            int val = Value;
            return val == 0 ? "default" : Convert.ToString(val);
        }
    }
}