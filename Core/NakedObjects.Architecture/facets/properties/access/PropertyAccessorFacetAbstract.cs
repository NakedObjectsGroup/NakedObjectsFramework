// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Facets.Properties.Access {
    public abstract class PropertyAccessorFacetAbstract : FacetAbstract, IPropertyAccessorFacet {
        protected PropertyAccessorFacetAbstract(IFacetHolder holder)
            : base(Type, holder) {}

        public static Type Type {
            get { return typeof (IPropertyAccessorFacet); }
        }

        #region IPropertyAccessorFacet Members

        public abstract object GetProperty(INakedObject nakedObject);

        #endregion
    }
}