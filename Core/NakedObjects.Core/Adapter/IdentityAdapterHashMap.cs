// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections;
using System.Collections.Generic;
using Common.Logging;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Core.Adapter {
    public class IdentityAdapterHashMap : IIdentityAdapterMap {
        private static readonly ILog Log;
        private readonly IDictionary<IOid, INakedObject> adapters;

        static IdentityAdapterHashMap() {
            Log = LogManager.GetLogger(typeof (IdentityAdapterHashMap));
        }

        public IdentityAdapterHashMap()
            : this(10) {}

        public IdentityAdapterHashMap(int capacity) {
            adapters = new Dictionary<IOid, INakedObject>(capacity);
        }

        #region IIdentityAdapterMap Members

        public virtual void Add(IOid oid, INakedObject adapter) {
            adapters[oid] = adapter;

            // log after so that adapter is in map if required by ToString
            Log.DebugFormat("Add {0} as {1}", oid, adapter);
        }

        public virtual INakedObject GetAdapter(IOid oid) {
            if (adapters.ContainsKey(oid)) {
                return adapters[oid];
            }
            return null;
        }

        public virtual bool IsIdentityKnown(IOid oid) {
            return adapters.ContainsKey(oid);
        }

        public virtual IEnumerator<IOid> GetEnumerator() {
            return adapters.Keys.GetEnumerator();
        }

        public virtual void Remove(IOid oid) {
            Log.DebugFormat("Remove {0}", oid);

            adapters.Remove(oid);
        }

        public virtual void Reset() {
            Log.Debug("Reset");

            adapters.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}