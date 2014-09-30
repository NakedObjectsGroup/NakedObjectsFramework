// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Callbacks;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Callbacks {
    public class OnUpdatingErrorCallbackFacetNull : OnUpdatingErrorCallbackFacetAbstract {
        public OnUpdatingErrorCallbackFacetNull(IFacetHolder holder)
            : base(holder) {}

        public override string Invoke(INakedObject nakedObject, Exception exception) {
            throw exception;
        }
    }
}