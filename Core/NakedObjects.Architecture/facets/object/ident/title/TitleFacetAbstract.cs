// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Facets.Objects.Ident.Title {
    public abstract class TitleFacetAbstract : FacetAbstract, ITitleFacet {
        protected TitleFacetAbstract(IFacetHolder holder)
            : base(Type, holder) {}

        public static Type Type {
            get { return typeof (ITitleFacet); }
        }

        #region ITitleFacet Members

        public virtual string GetTitleWithMask(string mask, INakedObject nakedObject) {
            return GetTitle(nakedObject);
        }

        public abstract string GetTitle(INakedObject nakedObject);

        #endregion
    }
}