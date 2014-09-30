// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Reflector.Spec {
    public abstract class NakedObjectMemberSessionAware : NakedObjectMemberAbstract {
        protected internal NakedObjectMemberSessionAware(string id, IFacetHolder facetHolder)
            : base(id, facetHolder) {}
    }


    // Copyright (c) Naked Objects Group Ltd.
}