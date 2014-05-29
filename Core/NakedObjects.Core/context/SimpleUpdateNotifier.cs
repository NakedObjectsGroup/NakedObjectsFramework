// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Context {
    public class SimpleUpdateNotifier : IUpdateNotifier {
        private static readonly ILog Log;
        private readonly IList<INakedObject> disposals = new List<INakedObject>();
        private IList<INakedObject> changes = new List<INakedObject>();

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

        public virtual void Init() {}

        public virtual void Shutdown() {
            Log.Info("  shutting down " + this);
            changes.Clear();
            changes = null;
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