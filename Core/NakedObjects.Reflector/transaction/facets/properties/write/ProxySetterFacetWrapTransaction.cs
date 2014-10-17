// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Facets.Properties.Modify;
using NakedObjects.Architecture.Resolve;

namespace NakedObjects.Reflector.Transaction.Facets.Properties.Write {
    public class ProxySetterFacetWrapTransaction : PropertySetterFacetAbstract {
        private readonly IPropertySetterFacet underlyingFacet;

        public ProxySetterFacetWrapTransaction(IPropertySetterFacet underlyingFacet)
            : base(underlyingFacet.Specification) {
            this.underlyingFacet = underlyingFacet;
        }

        public override void SetProperty(INakedObject inObject, INakedObject parameter, INakedObjectTransactionManager transactionManager) {
           
            if (inObject.ResolveState.IsPersistent()) {
                try {
                    transactionManager.StartTransaction();
                    underlyingFacet.SetProperty(inObject, parameter, transactionManager);
                    transactionManager.EndTransaction();
                }
                catch (Exception) {
                    transactionManager.AbortTransaction();
                    throw;
                }
            }
            else {
                underlyingFacet.SetProperty(inObject, parameter, transactionManager);
            }
        }

        public override string ToString() {
            return base.ToString() + " --> " + underlyingFacet;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}