// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections;
using System.Collections.Generic;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;

namespace NakedObjects.Core.Adapter.Map {
    public class PocoAdapterHashMap : IPocoAdapterMap {
        private static readonly ILog Log;
        private readonly IDictionary<object, INakedObject> domainObjects;

        static PocoAdapterHashMap() {
            Log = LogManager.GetLogger(typeof (PocoAdapterHashMap));
        }

        public PocoAdapterHashMap()
            : this(10) {}

        public PocoAdapterHashMap(int capacity) {
            domainObjects = new Dictionary<object, INakedObject>(capacity);
        }

        #region IPocoAdapterMap Members

        public virtual void Add(object obj, INakedObject adapter) {
            lock (domainObjects) {
                domainObjects[obj] = adapter;
            }
            // log at end so that if ToString needs adapters they're in maps. 
            Log.DebugFormat("Add instance of {0} as {1}", obj.GetType().FullName, adapter);
        }

        public virtual bool ContainsObject(object obj) {
            lock (domainObjects) {
                return domainObjects.ContainsKey(obj);
            }
        }

        public virtual IEnumerator<INakedObject> GetEnumerator() {
            return domainObjects.Values.GetEnumerator();
        }

        public virtual INakedObject GetObject(object obj) {
            lock (domainObjects) {
                if (ContainsObject(obj)) {
                    return domainObjects[obj];
                }
                return null;
            }
        }

        public virtual void Reset() {
            Log.Debug("Reset");
            lock (domainObjects) {
                domainObjects.Clear();
            }
        }

        public virtual void Shutdown() {
            Log.Debug("Shutdown");
            lock (domainObjects) {
                domainObjects.Clear();
            }
        }

        public virtual void Remove(INakedObject nakedObject) {
            Log.DebugFormat("Remove {0}", nakedObject);
            lock (domainObjects) {
                domainObjects.Remove(nakedObject.Object);
            }
        }

        #endregion

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}