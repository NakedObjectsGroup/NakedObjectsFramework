// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Callbacks;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Security;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Callbacks {
    public class UpdatedCallbackFacetNull : UpdatedCallbackFacetAbstract {
        public UpdatedCallbackFacetNull(IFacetHolder holder)
            : base(holder) {}

        public override void Invoke(INakedObject nakedObject, ISession session, INakedObjectPersistor persistor) {}
    }

    // Copyright (c) Naked Objects Group Ltd.
}