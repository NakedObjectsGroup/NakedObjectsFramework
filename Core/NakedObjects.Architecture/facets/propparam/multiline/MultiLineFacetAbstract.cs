// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Facets.Propparam.MultiLine {
    public abstract class MultiLineFacetAbstract : MultipleValueFacetAbstract, IMultiLineFacet {
        private readonly int numberOfLines;
        private readonly int width;

        protected MultiLineFacetAbstract(int numberOfLines, int width, IFacetHolder holder)
            : base(Type, holder) {
            this.numberOfLines = numberOfLines;
            this.width = width;
        }

        public static Type Type {
            get { return typeof (IMultiLineFacet); }
        }

        #region IMultiLineFacet Members

        public virtual int NumberOfLines {
            get { return numberOfLines; }
        }

        public virtual int Width {
            get { return width; }
        }

        #endregion

        protected override string ToStringValues() {
            return string.Format("lines={0}, width={1}", numberOfLines, width);
        }
    }
}