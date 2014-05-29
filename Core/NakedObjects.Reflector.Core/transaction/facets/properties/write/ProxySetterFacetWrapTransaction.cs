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
    public class ProxySetterFacetWrapTransaction : PropertySetterFacetAbstract {
        private readonly IPropertySetterFacet underlyingFacet;

        public ProxySetterFacetWrapTransaction(IPropertySetterFacet underlyingFacet)
            : base(underlyingFacet.FacetHolder) {
            this.underlyingFacet = underlyingFacet;
        }

        public override void SetProperty(INakedObject inObject, INakedObject parameter) {
            INakedObjectPersistor objectManager = NakedObjectsContext.ObjectPersistor;
            if (inObject.ResolveState.IsPersistent()) {
                try {
                    objectManager.StartTransaction();
                    underlyingFacet.SetProperty(inObject, parameter);
                    objectManager.EndTransaction();
                }
                catch (Exception) {
                    PersistorUtils.Abort(objectManager, FacetHolder);
                    throw;
                }
            }
            else {
                underlyingFacet.SetProperty(inObject, parameter);
            }
        }

        public override string ToString() {
            return base.ToString() + " --> " + underlyingFacet;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}