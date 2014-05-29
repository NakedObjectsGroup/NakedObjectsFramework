// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Util;

namespace NakedObjects.Core.Adapter.Map {
    public class MvcPocoAdapterHashMap : IPocoAdapterMap {
        private const string PocoAdapterMapName = "MvcPocoAdapterHashMap";
        private static readonly ILog Log;

        private static readonly IDictionary<object, INakedObject> TempMap = new Dictionary<object, INakedObject>();

        static MvcPocoAdapterHashMap() {
            Log = LogManager.GetLogger(typeof (PocoAdapterHashMap));
        }

        private static IDictionary<object, INakedObject> Map {
            get {
                if (HttpContext.Current == null || HttpContext.Current.Session == null) {
                    // ie no session available - expect this to be if fixtures are running. Just return a temporary 
                    // map to act as a holder while the fixtures are running. 
                    return TempMap;
                }
                if (HttpContext.Current.Session[PocoAdapterMapName] == null) {
                    HttpContext.Current.Session.Add(PocoAdapterMapName, new Dictionary<object, INakedObject>());
                }
                return HttpContext.Current.Session[PocoAdapterMapName] as IDictionary<object, INakedObject>;
            }
        }

        #region IPocoAdapterMap Members

        public virtual void Add(object obj, INakedObject adapter) {
            Map[obj] = adapter;

            // log at end so that if ToString needs adapters they're in maps. 
            Log.DebugFormat("Add instance of {0} as {1}", obj.GetType().FullName, adapter);
        }

        public virtual bool ContainsObject(object obj) {
            return Map.ContainsKey(obj);
        }

        public virtual IEnumerator<INakedObject> GetEnumerator() {
            return Map.Values.GetEnumerator();
        }


        public virtual INakedObject GetObject(object obj) {
            if (ContainsObject(obj)) {
                return Map[obj];
            }
            return null;
        }

        public virtual void Reset() {
            Log.Debug("Reset");

            KeyValuePair<object, INakedObject>[] toRemove = Map.Where(kvp => !kvp.Value.Oid.IsTransient).ToArray();
            toRemove.ForEach(kvp => Map.Remove(kvp.Key));
            TempMap.Clear();
        }

        public virtual void Shutdown() {
            Log.Debug("Shutdown");
            Map.Clear();
            TempMap.Clear();
        }

        public virtual void Remove(INakedObject nakedObject) {
            Log.DebugFormat("Remove {0}", nakedObject);
            Map.Remove(nakedObject.Object);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}