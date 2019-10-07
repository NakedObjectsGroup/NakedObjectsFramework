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
    public sealed class NakedObjectAdapterHashMap : INakedObjectAdapterMap {
        private static readonly ILog Log = LogManager.GetLogger(typeof (NakedObjectAdapterHashMap));
        private readonly IDictionary<object, INakedObjectAdapter> domainObjects;

        public NakedObjectAdapterHashMap(int capacity) {
            domainObjects = new Dictionary<object, INakedObjectAdapter>(capacity);
        }

        #region INakedObjectAdapterMap Members

        public void Add(object obj, INakedObjectAdapter adapter) {
            domainObjects[obj] = adapter;
        }

        public bool ContainsObject(object obj) {
            return domainObjects.ContainsKey(obj);
        }

        public IEnumerator<INakedObjectAdapter> GetEnumerator() {
            return domainObjects.Values.GetEnumerator();
        }

        public INakedObjectAdapter GetObject(object obj) {
            if (ContainsObject(obj)) {
                return domainObjects[obj];
            }
            return null;
        }

        public void Reset() {
            domainObjects.Clear();
        }

        public void Shutdown() {
            domainObjects.Clear();
        }

        public void Remove(INakedObjectAdapter nakedObjectAdapter) {
            domainObjects.Remove(nakedObjectAdapter.Object);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}