// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Facets.Objects.Callbacks {
    public abstract class UpdatingCallbackFacetAbstract : CallbackFacetAbstract, IUpdatingCallbackFacet {
        protected UpdatingCallbackFacetAbstract(IFacetHolder holder)
            : base(Type, holder) {}

        public static Type Type {
            get { return typeof (IUpdatingCallbackFacet); }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}