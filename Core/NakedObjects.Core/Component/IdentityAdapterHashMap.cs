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
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;

namespace NakedObjects.Core.Component {
    public sealed class IdentityAdapterHashMap : IIdentityAdapterMap {
        private readonly ILogger<FlatPersistAlgorithm> logger;
        private readonly IDictionary<IOid, INakedObjectAdapter> adapters;
        private readonly int capacity = 10;

        public IdentityAdapterHashMap() => adapters = new Dictionary<IOid, INakedObjectAdapter>(capacity);

        // used by DI
        // ReSharper disable once UnusedMember.Global
        public IdentityAdapterHashMap(IConfiguration config, 
                                      ILogger<FlatPersistAlgorithm> logger) : this() {
            this.logger = logger;
            var capacityFromConfig = config.GetSection("NakedObjects")["HashMapCapacity"];
            if (capacityFromConfig == null) {
                logger.LogWarning($"NakedObjects:HashMapCapacity not set defaulting to {capacity}");
            }
            else {
                capacity = int.Parse(capacityFromConfig);
            }

            adapters = new Dictionary<IOid, INakedObjectAdapter>(capacity);
        }

        #region IIdentityAdapterMap Members

        public void Add(IOid oid, INakedObjectAdapter adapter) => adapters[oid] = adapter;

        public INakedObjectAdapter GetAdapter(IOid oid) => adapters.ContainsKey(oid) ? adapters[oid] : null;

        public bool IsIdentityKnown(IOid oid) => adapters.ContainsKey(oid);

        public IEnumerator<IOid> GetEnumerator() => adapters.Keys.GetEnumerator();

        public void Remove(IOid oid) => adapters.Remove(oid);

        public void Reset() => adapters.Clear();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}