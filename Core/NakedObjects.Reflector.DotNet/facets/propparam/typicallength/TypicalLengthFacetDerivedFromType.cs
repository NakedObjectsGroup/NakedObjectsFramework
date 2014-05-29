// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets.Objects.TypicalLength;

namespace NakedObjects.Architecture.Facets.Propparam.TypicalLength {
    public class TypicalLengthFacetDerivedFromType : TypicalLengthFacetAbstract {
        public TypicalLengthFacetDerivedFromType(ITypicalLengthFacet typicalLengthFacet, IFacetHolder holder)
            : base(typicalLengthFacet.Value, holder) {}
    }
}