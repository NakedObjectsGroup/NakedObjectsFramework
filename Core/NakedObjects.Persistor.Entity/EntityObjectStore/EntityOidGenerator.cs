// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Threading;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Util;

namespace NakedObjects.EntityObjectStore {
    public class EntityOidGenerator : IOidGenerator {
        private readonly IMetamodelManager metamodel;
        private static readonly ILog Log = LogManager.GetLogger(typeof (EntityOidGenerator));
        private static long transientId;

        public EntityOidGenerator(IMetamodelManager metamodel) {
            Assert.AssertNotNull(metamodel);
            this.metamodel = metamodel;
        }

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
            var oid = new EntityOid(metamodel, obj.GetType(), new object[] { Interlocked.Increment(ref transientId) }, true);
            Log.DebugFormat("Created OID {0} for instance of {1}", oid, obj.GetType().FullName);
            return oid;
        }

        

        public IOid RestoreOid(string[] encodedData) {
            return new EntityOid(metamodel, encodedData);
        }

        public IOid CreateOid(string typeName, object[] keys) {
            return new EntityOid(metamodel, typeName, keys);
        }

        #endregion
    }
}