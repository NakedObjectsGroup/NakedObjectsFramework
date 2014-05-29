// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Ident.Title;
using NakedObjects.Capabilities;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Ident.Title {
    public class TitleFacetUsingParser<T> : FacetAbstract, ITitleFacet {
        private readonly IParser<T> parser;

        public TitleFacetUsingParser(IParser<T> parser, IFacetHolder holder)
            : base(typeof (ITitleFacet), holder) {
            this.parser = parser;
        }

        #region ITitleFacet Members

        public string GetTitle(INakedObject nakedObject) {
            if (nakedObject == null || nakedObject.Object == null) {
                return null;
            }
            return parser.DisplayTitleOf((T) nakedObject.Object);
        }

        public string GetTitleWithMask(string mask, INakedObject nakedObject) {
            if (nakedObject == null || nakedObject.Object == null) {
                return null;
            }
            return parser.TitleWithMaskOf(mask, (T) nakedObject.Object);
        }

        #endregion

        protected override string ToStringValues() {
            return parser.ToString();
        }
    }
}