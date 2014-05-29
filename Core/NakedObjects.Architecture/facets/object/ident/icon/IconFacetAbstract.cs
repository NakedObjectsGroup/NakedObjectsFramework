// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Facets.Objects.Ident.Icon {
    public abstract class IconFacetAbstract : FacetAbstract, IIconFacet {
        protected IconFacetAbstract(IFacetHolder holder)
            : base(Type, holder) {}

        public static Type Type {
            get { return typeof (IIconFacet); }
        }

        #region IIconFacet Members

        public abstract string GetIconName(INakedObject nakedObject);

        public abstract string GetIconName();

        #endregion
    }
}