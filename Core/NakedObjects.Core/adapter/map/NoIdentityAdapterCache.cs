// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Core.Adapter.Map {
    public class NoIdentityAdapterCache : INoIdentityAdapterCache {
        private readonly Dictionary<object, INakedObject> adapters = new Dictionary<object, INakedObject>();

        #region INoIdentityAdapterCache Members

        public void AddAdapter(INakedObject adapter) {
            adapters[adapter.Object] = adapter;
        }

        public INakedObject GetAdapter(object domainObject) {
            if (adapters.ContainsKey(domainObject)) {
                return adapters[domainObject];
            }
            return null;
        }

        public void Reset() {
            adapters.Clear();
        }

        #endregion
    }
}