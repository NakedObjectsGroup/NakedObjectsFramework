// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Facets.Properties.Defaults {
    public abstract class PropertyDefaultFacetAbstract : FacetAbstract, IPropertyDefaultFacet {
        protected PropertyDefaultFacetAbstract(IFacetHolder holder)
            : base(Type, holder) {}

        public static Type Type {
            get { return typeof (IPropertyDefaultFacet); }
        }

        #region IPropertyDefaultFacet Members

        public abstract object GetDefault(INakedObject nakedObject);

        #endregion
    }


    // Copyright (c) Naked Objects Group Ltd.
}