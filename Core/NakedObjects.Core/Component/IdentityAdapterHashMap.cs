// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections;
using System.Collections.Generic;
using Common.Logging;
using Microsoft.Extensions.Configuration;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;

namespace NakedObjects.Core.Component {
    public sealed class IdentityAdapterHashMap : IIdentityAdapterMap {
        private static readonly ILog Log;
        private readonly IDictionary<IOid, INakedObjectAdapter> adapters;
        private readonly int capacity = 10;

        static IdentityAdapterHashMap() {
            Log = LogManager.GetLogger(typeof(IdentityAdapterHashMap));
        }

        public IdentityAdapterHashMap() {
            adapters = new Dictionary<IOid, INakedObjectAdapter>(capacity);
        }

        public IdentityAdapterHashMap(IConfiguration config) : this() {
            var capacityFromConfig = config.GetSection("NakedObjects")["HashMapCapacity"];
            if (capacityFromConfig == null) {
                Log.Warn($"NakedObjects:HashMapCapacity not set defaulting to {capacity}");
            }
            else {
                capacity = int.Parse(capacityFromConfig);
            }

            adapters = new Dictionary<IOid, INakedObjectAdapter>(capacity);
        }

        #region IIdentityAdapterMap Members

        public void Add(IOid oid, INakedObjectAdapter adapter) {
            adapters[oid] = adapter;
        }

        public INakedObjectAdapter GetAdapter(IOid oid) {
            if (adapters.ContainsKey(oid)) {
                return adapters[oid];
            }

            return null;
        }

        public bool IsIdentityKnown(IOid oid) {
            return adapters.ContainsKey(oid);
        }

        public IEnumerator<IOid> GetEnumerator() {
            return adapters.Keys.GetEnumerator();
        }

        public void Remove(IOid oid) {
            adapters.Remove(oid);
        }

        public void Reset() {
            adapters.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}