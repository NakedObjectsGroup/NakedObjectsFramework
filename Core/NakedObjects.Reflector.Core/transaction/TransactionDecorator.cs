// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actions.Invoke;
using NakedObjects.Architecture.Facets.Properties.Modify;
using NakedObjects.Reflector.Spec;
using NakedObjects.Reflector.Transaction.Facets.Actions.Invoke;
using NakedObjects.Reflector.Transaction.Facets.Properties.Write;

namespace NakedObjects.Reflector.Transaction {
    public class TransactionDecorator : IFacetDecorator {
        #region IFacetDecorator Members

        public virtual IFacet Decorate(IFacet facet, IFacetHolder holder) {
            if (facet.FacetType == typeof (IActionInvocationFacet)) {
                return new ActionInvocationFacetWrapTransaction(((IActionInvocationFacet) facet));
            }

            if (facet.FacetType == typeof (IPropertyClearFacet)) {
                return new PropertyClearFacetWrapTransaction(((IPropertyClearFacet) facet));
            }

            if (facet.FacetType == typeof (IPropertySetterFacet)) {
                return new ProxySetterFacetWrapTransaction(((IPropertySetterFacet) facet));
            }

            return facet;
        }

        public virtual Type[] ForFacetTypes {
            get { return new[] {typeof (IActionInvocationFacet), typeof (IPropertyClearFacet), typeof (IPropertySetterFacet)}; }
        }

        #endregion
    }


    // Copyright (c) Naked Objects Group Ltd.
}