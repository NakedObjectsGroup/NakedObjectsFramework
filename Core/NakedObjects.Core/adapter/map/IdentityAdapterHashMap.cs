// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections;
using System.Collections.Generic;
using Common.Logging;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Core.Adapter.Map {
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
            lock (adapters) {
                adapters[oid] = adapter;
            }
            // log after so that adapter is in map if required by ToString
            Log.DebugFormat("Add {0} as {1}", oid, adapter);
        }

        public virtual INakedObject GetAdapter(IOid oid) {
            lock (adapters) {
                if (adapters.ContainsKey(oid)) {
                    return adapters[oid];
                }
                return null;
            }
        }

        public virtual bool IsIdentityKnown(IOid oid) {
            lock (adapters) {
                return adapters.ContainsKey(oid);
            }
        }

        public virtual IEnumerator<IOid> GetEnumerator() {
            lock (adapters) {
                return adapters.Keys.GetEnumerator();
            }
        }

        public virtual void Remove(IOid oid) {
            Log.DebugFormat("Remove {0}", oid);
            lock (adapters) {
                adapters.Remove(oid);
            }
        }

        public virtual void Reset() {
            Log.Debug("Reset");
            lock (adapters) {
                adapters.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}