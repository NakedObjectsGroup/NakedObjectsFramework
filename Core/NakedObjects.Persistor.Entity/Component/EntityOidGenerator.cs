// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Persistor.Entity.Adapter;

namespace NakedObjects.Persistor.Entity.Component {
    public sealed class EntityOidGenerator : IOidGenerator {
        private static long transientId;
        private readonly ILoggerFactory loggerFactory;
        private readonly IMetamodelManager metamodel;

        public EntityOidGenerator(IMetamodelManager metamodel, ILoggerFactory loggerFactory) {
            this.metamodel = metamodel ?? throw new InitialisationException($"{nameof(metamodel)} is null");
            this.loggerFactory = loggerFactory ?? throw new InitialisationException($"{nameof(loggerFactory)} is null");
        }

        public string Name => "Entity Oids";

        #region IOidGenerator Members

        public void ConvertTransientToPersistentOid(IOid oid) => (oid as IEntityOid)?.MakePersistent();

        public IOid CreateTransientOid(object obj) => new EntityOid(metamodel, obj.GetType(), new object[] {++transientId}, true, Logger());

        public IOid RestoreOid(string[] encodedData) => new EntityOid(metamodel, loggerFactory, encodedData);

        public IOid CreateOid(string typeName, object[] keys) => new EntityOid(metamodel, typeName, keys, Logger());

        #endregion

        private ILogger<EntityOid> Logger() => loggerFactory.CreateLogger<EntityOid>();
    }
}