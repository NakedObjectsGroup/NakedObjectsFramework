// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;

namespace NakedObjects.Architecture.Adapter.Value {
    public interface IDoubleFloatingPointValueFacet : IFacet {
        double DoubleValue(INakedObject nakedObject);
    }

    // Copyright (c) Naked Objects Group Ltd.
}