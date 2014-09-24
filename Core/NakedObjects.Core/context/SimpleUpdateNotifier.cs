// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Context {
    public class SimpleUpdateNotifier : IUpdateNotifier {
        private static readonly ILog Log;
        private readonly IList<INakedObject> changes = new List<INakedObject>();
        private readonly IList<INakedObject> disposals = new List<INakedObject>();

        static SimpleUpdateNotifier() {
            Log = LogManager.GetLogger(typeof (SimpleUpdateNotifier));
        }

        #region IUpdateNotifier Members

        public virtual void AddChangedObject(INakedObject nakedObject) {
            lock (changes) {
                Log.DebugFormat("Mark as dirty {0}", nakedObject);
                if (!changes.Contains(nakedObject)) {
                    changes.Add(nakedObject);
                }
            }
        }

        public virtual void AddDisposedObject(INakedObject nakedObject) {
            lock (disposals) {
                Log.DebugFormat("Mark as disposed {0}", nakedObject);
                if (!disposals.Contains(nakedObject)) {
                    disposals.Add(nakedObject);
                }
            }
        }

        public virtual IEnumerator<INakedObject> AllChangedObjects() {
            lock (changes) {
                List<INakedObject> changedObjects = changes.ToList();
                changes.Clear();
                if (changedObjects.Count > 0) {
                    Log.DebugFormat("Dirty objects {0}", CollectionItemsToString(changedObjects));
                }

                return changedObjects.GetEnumerator();
            }
        }

        public virtual IEnumerator<INakedObject> AllDisposedObjects() {
            lock (disposals) {
                List<INakedObject> disposedObjects = disposals.ToList();
                disposals.Clear();
                if (disposedObjects.Count > 0) {
                    Log.DebugFormat("Disposed objects {0}", CollectionItemsToString(disposedObjects));
                }

                return disposedObjects.GetEnumerator();
            }
        }

        public virtual void EnsureEmpty() {
            if (changes.Count > 0) {
                throw new InvalidStateException("Update notifier still has updates");
            }
        }

        #endregion

        private static string CollectionItemsToString(IEnumerable<INakedObject> collection) {
            var result = new StringBuilder();
            foreach (INakedObject nakedObject in collection) {
                result.Append(nakedObject).Append(" ");
            }
            return result.ToString();
        }

        public override string ToString() {
            return new AsString(this).Append("changes", changes).ToString();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}