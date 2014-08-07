// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Properties.Modify;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Core.Context;
using NakedObjects.Core.Persist;

namespace NakedObjects.Reflector.Transaction.Facets.Properties.Write {
    public class PropertyClearFacetWrapTransaction : PropertyClearFacetAbstract {
        private readonly IPropertyClearFacet underlyingFacet;

        public PropertyClearFacetWrapTransaction(IPropertyClearFacet underlyingFacet)
            : base(underlyingFacet.FacetHolder) {
            this.underlyingFacet = underlyingFacet;
        }

        public override void ClearProperty(INakedObject inObject) {
            INakedObjectPersistor objectManager = NakedObjectsContext.ObjectPersistor;
            if (inObject.ResolveState.IsPersistent()) {
                try {
                    objectManager.StartTransaction();
                    underlyingFacet.ClearProperty(inObject);
                    objectManager.EndTransaction();
                }
                catch (Exception) {
                    NakedObjectsContext.ObjectPersistor.Abort(objectManager, FacetHolder);
                    throw;
                }
            }
            else {
                underlyingFacet.ClearProperty(inObject);
            }
        }

        public override string ToString() {
            return base.ToString() + " --> " + underlyingFacet;
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}