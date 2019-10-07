// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity.Core;
using System.Linq;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Util;

namespace NakedObjects.Persistor.Entity.Adapter {
    public sealed class EntityOid : IEncodedToStrings, IEntityOid {
        private static readonly ILog Log = LogManager.GetLogger(typeof(EntityOid));

        private readonly IMetamodelManager metamodel;
        private int cachedHashCode;
        private string cachedToString;
        private EntityOid previous;
        public EntityKey EntityKey { get; set; }

        #region IEncodedToStrings Members

        public string[] ToEncodedStrings() {
            var helper = new StringEncoderHelper();

            helper.Add(TypeName);
            helper.Add(Key);
            helper.Add(IsTransient);
            helper.AddSerializable(EntityKey);
            helper.Add(HasPrevious);
            if (HasPrevious) {
                helper.Add(previous as IEncodedToStrings);
            }

            return helper.ToArray();
        }

        public string[] ToShortEncodedStrings() {
            var helper = new StringEncoderHelper();

            helper.Add(TypeName);
            helper.Add(Key);
            helper.Add(IsTransient);
            helper.AddSerializable(EntityKey);

            return helper.ToArray();
        }

        #endregion

        #region IEntityOid Members

        public string TypeName { get; private set; }
        public object[] Key { get; private set; }

        public void CopyFrom(IOid oid) {
            Assert.AssertTrue("Copy from Oid must be Entity Oid", oid is EntityOid);
            var from = (EntityOid) oid;
            Key = from.Key;
            TypeName = from.TypeName;
            EntityKey = from.EntityKey;
            IsTransient = from.IsTransient;
            CacheState();
        }

        public ITypeSpec Spec => metamodel.GetSpecification(TypeNameUtils.DecodeTypeName(TypeName));

        public IOid Previous => previous;

        public bool HasPrevious => previous != null;

        public bool IsTransient { get; private set; }

        public void MakePersistent() {
            ThrowErrorIfNotTransient();
            previous = new EntityOid(metamodel, TypeName, Key) {IsTransient = IsTransient};
            IsTransient = false;
            CacheState();
        }

        public void MakePersistentAndUpdateKey(object[] newKey) {
            ThrowErrorIfNotTransient(newKey);
            previous = new EntityOid(metamodel, TypeName, Key) {IsTransient = IsTransient};
            Key = newKey; // after old key is saved ! 
            IsTransient = false;
            CacheState();
        }

        #endregion

        private void CacheState() {
            cachedHashCode = HashCodeUtils.Seed;
            cachedHashCode = HashCodeUtils.Hash(cachedHashCode, TypeName);
            cachedHashCode = HashCodeUtils.Hash(cachedHashCode, Key);

            object keys = Key.Aggregate((s, t) => s + ":" + t);

            cachedToString = $"{(IsTransient ? "T" : "")}EOID#{keys}{(previous == null ? "" : "+")}";
        }

        private void ThrowErrorIfNotTransient(object[] newKey = null) {
            if (!IsTransient) {
                string newKeyString = newKey != null ? newKey.Aggregate("New Key", (s, t) => s + " : " + t.ToString()) : "";
                string error = $"Attempting to make persistent an already persisted object. Type {TypeName}  Existing Key: {cachedToString} {newKeyString}";
                throw new NotPersistableException(Log.LogAndReturn(error));
            }
        }

        #region Constructors

        public EntityOid(IMetamodelManager metamodel, Type type, object[] key, bool isTransient)
            : this(metamodel, type.FullName, key) {
            IsTransient = isTransient;
            CacheState();
        }

        public EntityOid(IMetamodelManager metamodel, Type type, object[] key) : this(metamodel, type.FullName, key) { }

        public EntityOid(IMetamodelManager metamodel, string typeName, object[] key) {
            Assert.AssertNotNull(metamodel);
            this.metamodel = metamodel;
            TypeName = TypeNameUtils.EncodeTypeName(typeName);
            Key = key;
            IsTransient = false;
            CacheState();
        }

        public EntityOid(IMetamodelManager metamodel, object pojo, object[] key) : this(metamodel, pojo.GetType(), key) { }

        public EntityOid(IMetamodelManager metamodel, string[] strings) {
            Assert.AssertNotNull(metamodel);
            this.metamodel = metamodel;
            var helper = new StringDecoderHelper(metamodel, strings);

            TypeName = helper.GetNextString();
            Key = helper.GetNextObjectArray();
            IsTransient = helper.GetNextBool();
            EntityKey = (EntityKey) helper.GetNextSerializable();

            if (helper.HasNext) {
                bool hasPrevious = helper.GetNextBool();
                if (hasPrevious) {
                    previous = (EntityOid) helper.GetNextEncodedToStrings();
                }
            }

            CacheState();
        }

        #endregion

        #region Object Overrides

        public override bool Equals(object obj) {
            if (obj == this) {
                return true;
            }

            var oid = obj as EntityOid;
            if (oid != null) {
                return TypeName.Equals(oid.TypeName) && Key.SequenceEqual(oid.Key);
            }

            return false;
        }

        // ReSharper disable once NonReadonlyFieldInGetHashCode
        public override int GetHashCode() {
            return cachedHashCode;
        }

        public override string ToString() {
            return cachedToString;
        }

        #endregion
    }
}