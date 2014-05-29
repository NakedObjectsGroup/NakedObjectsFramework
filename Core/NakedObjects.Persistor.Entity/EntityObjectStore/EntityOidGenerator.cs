// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Threading;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Core.Persist;

namespace NakedObjects.EntityObjectStore {
    public class EntityOidGenerator : IOidGenerator {
        private static readonly ILog Log = LogManager.GetLogger(typeof (EntityOidGenerator));
        private static long transientId;

        public string Name {
            get { return "Entity Oids"; }
        }

        #region IOidGenerator Members

        public void ConvertPersistentToTransientOid(IOid oid) {}

        public void ConvertTransientToPersistentOid(IOid oid) {
            var entityOid = oid as EntityOid;
            if (entityOid != null) {
                entityOid.MakePersistent();
            }
            Log.DebugFormat("Converted transient OID to persistent {0}", oid);
        }

        public IOid CreateTransientOid(object obj) {
            var oid = new EntityOid(obj.GetType(), new object[] {Interlocked.Increment(ref transientId)}, true);
            Log.DebugFormat("Created OID {0} for instance of {1}", oid, obj.GetType().FullName);
            return oid;
        }

        public void Init() {}

        public void Shutdown() {}

        public IOid RestoreOid(string[] encodedData) {
            return PersistorUtils.RestoreGenericOid(encodedData) ?? new EntityOid(encodedData);
        }

        public EntityOid CreateOid(string typeName, object[] keys) {
            return new EntityOid(typeName, keys);
        }

        #endregion
    }
}