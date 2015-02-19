// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity.Core;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Util;

namespace NakedObjects.Persistor.Entity.Adapter {
    internal class EntityOid : IEncodedToStrings, IEntityOid {
        private readonly IMetamodelManager metamodel;
        private readonly int cachedHashCode;
        private readonly string cachedToString;
        private readonly EntityOid previous;

        #region Constructors

        private EntityOid(EntityOid eoid) {
            Assert.AssertNotNull(eoid);
            metamodel = eoid.metamodel;
            TypeName = eoid.TypeName;
            Key = eoid.Key;
            IsTransient = eoid.IsTransient;

            var cache = CacheState();
            cachedHashCode = cache.Item1;
            cachedToString = cache.Item2;
        }


        private EntityOid(EntityOid eoid, EntityOid previous, bool isTransient) {
            Assert.AssertNotNull(eoid);
            metamodel = eoid.metamodel;
            TypeName = eoid.TypeName;
            Key = eoid.Key;
            IsTransient = isTransient;
            this.previous = previous;
            var cache = CacheState();
            cachedHashCode = cache.Item1;
            cachedToString = cache.Item2;
        }

        private EntityOid(IMetamodelManager metamodel, string typeName, object[] key, EntityOid previous,  bool isTransient) {
            this.metamodel = metamodel;
            TypeName = typeName;
            Key = key;
            this.previous = previous;
            IsTransient = isTransient;
            var cache = CacheState();
            cachedHashCode = cache.Item1;
            cachedToString = cache.Item2;
        }

        public EntityOid(IMetamodelManager metamodel, Type type, object[] key, bool isTransient)
            : this(metamodel, type.FullName, key) {
            IsTransient = isTransient;
            var cache = CacheState();
            cachedHashCode = cache.Item1;
            cachedToString = cache.Item2;
        }

        public EntityOid(IMetamodelManager metamodel, Type type, object[] key) : this(metamodel, type.FullName, key) {}

        public EntityOid(IMetamodelManager metamodel, string typeName, object[] key) {
            Assert.AssertNotNull(metamodel);
            this.metamodel = metamodel;
            TypeName = TypeNameUtils.EncodeTypeName(typeName);
            Key = key;
            IsTransient = false;
            var cache = CacheState();
            cachedHashCode = cache.Item1;
            cachedToString = cache.Item2;
        }

        public EntityOid(IMetamodelManager metamodel, object pojo, object[] key) : this(metamodel, pojo.GetType(), key) {}

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
            var cache = CacheState();
            cachedHashCode = cache.Item1;
            cachedToString = cache.Item2;
        }

        #endregion

        public string TypeName { get; private set; }
        public object[] Key { get; private set; }
        public EntityKey EntityKey { get; private set; }

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

        #region IOid Members

        public IOid CopyFrom(IOid oid) {
            Assert.AssertTrue("Copy from Oid must be Entity Oid", oid is EntityOid);
            var from = (EntityOid) oid;
            return new EntityOid(from);
        }

        public ITypeSpec Spec {
            get { return metamodel.GetSpecification(TypeNameUtils.DecodeTypeName(TypeName)); }
        }

        public IOid Previous {
            get { return previous; }
        }

        public bool HasPrevious {
            get { return previous != null; }
        }

        public bool IsTransient { get; private set; }

        #endregion

        private Tuple<int, string> CacheState() {
            var hashCode = HashCodeUtils.Seed;
            hashCode = HashCodeUtils.Hash(hashCode, TypeName);
            hashCode = HashCodeUtils.Hash(hashCode, Key);

            object keys = Key.Aggregate((s, t) => s + ":" + t);

            var toString = string.Format("{0}EOID#{1}{2}", IsTransient ? "T" : "", keys, previous == null ? "" : "+");

            return new Tuple<int, string>(hashCode, toString);
        }

        public IOid MakePersistent() {
            ThrowErrorIfNotTransient();
            return new EntityOid(this, this, false);
        }

        private void ThrowErrorIfNotTransient(object[] newKey = null) {
            if (!IsTransient) {
                string newKeyString = newKey != null ? newKey.Aggregate("New Key", (s, t) => s + " : " + t.ToString()) : "";
                string error = string.Format("Attempting to make persistent an already persisted object. Existing Key: {0} {1}", ToString(), newKeyString);
                Assert.AssertTrue(error, IsTransient);
            }
        }

        public IOid MakePersistentAndUpdateKey(object[] newKey) {
            ThrowErrorIfNotTransient(newKey);
            return new EntityOid(metamodel, TypeName, newKey, this, false);
        }

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

        public override int GetHashCode() {
            return cachedHashCode;
        }

        public override string ToString() {
            return cachedToString;
        }

        #endregion
    }
}