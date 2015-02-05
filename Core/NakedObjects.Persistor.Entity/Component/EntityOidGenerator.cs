// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Util;
using NakedObjects.Persistor.Entity.Adapter;

namespace NakedObjects.Persistor.Entity {
    public class EntityOidGenerator : IOidGenerator {
        private static readonly ILog Log = LogManager.GetLogger(typeof (EntityOidGenerator));
        private static long transientId;
        private readonly IMetamodelManager metamodel;

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
            var entityOid = oid as IEntityOid;
            if (entityOid != null) {
                entityOid.MakePersistent();
            }
            Log.DebugFormat("Converted transient OID to persistent {0}", oid);
        }

        public IOid CreateTransientOid(object obj) {
            var oid = new EntityOid(metamodel, obj.GetType(), new object[] {++transientId}, true);
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