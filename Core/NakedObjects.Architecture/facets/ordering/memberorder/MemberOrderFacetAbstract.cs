// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Facets.Ordering.MemberOrder {
    public abstract class MemberOrderFacetAbstract : MultipleValueFacetAbstract, IMemberOrderFacet {
        private readonly string name;
        private readonly string sequence;

        protected MemberOrderFacetAbstract(string name, string sequence, IFacetHolder holder)
            : base(Type, holder) {
            this.name = name;
            this.sequence = sequence;
        }

        public static Type Type {
            get { return typeof (IMemberOrderFacet); }
        }

        #region IMemberOrderFacet Members

        public virtual string Name {
            get { return name; }
        }

        public virtual string Sequence {
            get { return sequence; }
        }

        #endregion
    }
}