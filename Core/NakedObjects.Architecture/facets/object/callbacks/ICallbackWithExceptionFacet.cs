// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Facets.Objects.Callbacks {
    public interface ICallbackWithExceptionFacet : IFacet {
        string Invoke(INakedObject nakedObject, Exception exception);
    }
}