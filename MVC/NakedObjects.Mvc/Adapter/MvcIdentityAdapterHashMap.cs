// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Adapter.Map {
    public class MvcIdentityAdapterHashMap : IIdentityAdapterMap {
        private const string IdentityMapName = "MvcIdentityAdapterHashMap";
        private static readonly ILog Log;
        private static readonly IDictionary<IOid, INakedObject> TempMap = new Dictionary<IOid, INakedObject>();

        static MvcIdentityAdapterHashMap() {
            Log = LogManager.GetLogger(typeof (MvcIdentityAdapterHashMap));
        }

        public static bool StoringTransientsInSession {
            get { return HttpContext.Current != null && HttpContext.Current.Session[IdentityMapName] != null; }
        }

        private static IDictionary<IOid, INakedObject> Map {
            get {
                if (HttpContext.Current == null || HttpContext.Current.Session == null) {
                    // ie no session available - expect this to be if fixtures are running. Just return a temporary 
                    // map to act as a holder while the fixtures are running. 
                    return TempMap;
                }
                if (HttpContext.Current.Session[IdentityMapName] == null) {
                    HttpContext.Current.Session.Add(IdentityMapName, new Dictionary<IOid, INakedObject>());
                }
                return HttpContext.Current.Session[IdentityMapName] as IDictionary<IOid, INakedObject>;
            }
        }

        #region IIdentityAdapterMap Members

        public virtual void Add(IOid oid, INakedObject adapter) {
            Map[oid] = adapter;

            // log after so that adapter is in map if required by ToString
            Log.DebugFormat("Add {0} as {1}", oid, adapter);
        }


        public virtual INakedObject GetAdapter(IOid oid) {
            if (Map.ContainsKey(oid)) {
                return Map[oid];
            }
            return null;
        }


        public virtual bool IsIdentityKnown(IOid oid) {
            return Map.ContainsKey(oid);
        }

        public virtual IEnumerator<IOid> GetEnumerator() {
            return Map.Keys.GetEnumerator();
        }

        public virtual void Remove(IOid oid) {
            Map.Remove(oid);
            Log.DebugFormat("Remove {0}", oid);
        }

        public virtual void Reset() {
            Log.Debug("Reset");
            IEnumerable<IOid> toRemove = Map.Keys.Where(k => !k.IsTransient).ToArray();
            toRemove.ForEach(k => Map.Remove(k));
            TempMap.Clear();
        }

        public virtual void Shutdown() {
            Log.Debug("Shutdown");
            Map.Clear();
            TempMap.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}