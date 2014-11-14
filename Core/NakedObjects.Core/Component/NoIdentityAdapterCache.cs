// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Core.Adapter;

namespace NakedObjects.Core.Component {
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