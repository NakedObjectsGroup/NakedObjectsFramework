// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Facets.Properties.Modify {
    public abstract class PropertyInitializationFacetAbstract : FacetAbstract, IPropertyInitializationFacet {
        protected PropertyInitializationFacetAbstract(IFacetHolder holder)
            : base(Type, holder) {}

        public static Type Type {
            get { return typeof (IPropertyInitializationFacet); }
        }

        #region IPropertyInitializationFacet Members

        public
            abstract void InitProperty(INakedObject nakedObject, INakedObject nakedValue);

        #endregion
    }
}