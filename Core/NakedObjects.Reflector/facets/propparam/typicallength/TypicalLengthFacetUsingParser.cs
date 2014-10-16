// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.TypicalLength;
using NakedObjects.Capabilities;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.TypicalLength {
    public class TypicalLengthFacetUsingParser<T> : FacetAbstract, ITypicalLengthFacet {
        private readonly IParser<T> parser;

        public TypicalLengthFacetUsingParser(IParser<T> parser, ISpecification holder)
            : base(typeof (ITypicalLengthFacet), holder) {
            this.parser = parser;
        }

        #region ITypicalLengthFacet Members

        public int Value {
            get { return parser.TypicalLength; }
        }

        #endregion

        protected override string ToStringValues() {
            return parser.ToString();
        }
    }
}