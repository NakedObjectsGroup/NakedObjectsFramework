// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;

namespace NakedObjects.Reflector.Spec {
    public interface IFacetDecorator {
        Type[] ForFacetTypes { get; }
        IFacet Decorate(IFacet facet, IFacetHolder holder);
    }

    // Copyright (c) Naked Objects Group Ltd.
}