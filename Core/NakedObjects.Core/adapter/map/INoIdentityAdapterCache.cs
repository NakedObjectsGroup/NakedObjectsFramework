// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Core.Adapter.Map {
    public interface INoIdentityAdapterCache {
        void AddAdapter(INakedObject adapter);
        INakedObject GetAdapter(object domainObject);
        void Reset();
    }
}