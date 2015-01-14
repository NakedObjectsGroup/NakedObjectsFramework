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
using NakedObjects.Architecture.Component;

namespace NakedObjects.Core.Component {
    public class PocoAdapterHashMap : IPocoAdapterMap {
        private static readonly ILog Log = LogManager.GetLogger(typeof (PocoAdapterHashMap));
        private readonly IDictionary<object, INakedObject> domainObjects;

        public PocoAdapterHashMap(int capacity) {
            domainObjects = new Dictionary<object, INakedObject>(capacity);
        }

        #region IPocoAdapterMap Members

        public virtual void Add(object obj, INakedObject adapter) {
            domainObjects[obj] = adapter;

            // log at end so that if ToString needs adapters they're in maps. 
            Log.DebugFormat("Add instance of {0} as {1}", obj.GetType().FullName, adapter);
        }

        public virtual bool ContainsObject(object obj) {
            return domainObjects.ContainsKey(obj);
        }

        public virtual IEnumerator<INakedObject> GetEnumerator() {
            return domainObjects.Values.GetEnumerator();
        }

        public virtual INakedObject GetObject(object obj) {
            if (ContainsObject(obj)) {
                return domainObjects[obj];
            }
            return null;
        }

        public virtual void Reset() {
            Log.Debug("Reset");

            domainObjects.Clear();
        }

        public virtual void Shutdown() {
            Log.Debug("Shutdown");

            domainObjects.Clear();
        }

        public virtual void Remove(INakedObject nakedObject) {
            Log.DebugFormat("Remove {0}", nakedObject);

            domainObjects.Remove(nakedObject.Object);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}