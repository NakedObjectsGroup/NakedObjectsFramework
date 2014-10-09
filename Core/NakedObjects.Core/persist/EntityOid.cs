// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Data.Entity.Core;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Util;

namespace NakedObjects.EntityObjectStore {
    public class EntityOid : IOid, IEncodedToStrings {
        private readonly IMetamodelManager metamodel;
        private int cachedHashCode;
        private string cachedToString;
        private EntityOid previous;

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

        public string TypeName { get; private set; }
        public object[] Key { get; private set; }
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

        #region IOid Members

        public void CopyFrom(IOid oid) {
            Assert.AssertTrue("Copy from Oid must be Entity Oid", oid is EntityOid);
            var from = (EntityOid) oid;
            Key = from.Key;
            TypeName = from.TypeName;
            EntityKey = from.EntityKey;
            IsTransient = from.IsTransient;
            CacheState();
        }

        public INakedObjectSpecification Specification {
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

        private void CacheState() {
            cachedHashCode = HashCodeUtils.Seed;
            cachedHashCode = HashCodeUtils.Hash(cachedHashCode, TypeName);
            cachedHashCode = HashCodeUtils.Hash(cachedHashCode, Key);

            object keys = Key.Aggregate((s, t) => s + ":" + t);

            cachedToString = string.Format("{0}EOID#{1}{2}", IsTransient ? "T" : "", keys, previous == null ? "" : "+");
        }

        public void MakePersistent() {
            ThrowErrorIfNotTransient();
            previous = new EntityOid(metamodel, TypeName, Key) {IsTransient = IsTransient};
            IsTransient = false;
            CacheState();
        }

        private void ThrowErrorIfNotTransient(object[] newKey = null) {
            if (!IsTransient) {
                string newKeyString = newKey != null ? newKey.Aggregate("New Key", (s, t) => s + " : " + t.ToString()) : "";
                string error = string.Format("Attempting to make persistent an already persisted object. Existing Key: {0} {1}", ToString(), newKeyString);
                Assert.AssertTrue(error, IsTransient);
            }
        }

        public void MakePersistentAndUpdateKey(object[] newKey) {
            ThrowErrorIfNotTransient(newKey);
            previous = new EntityOid(metamodel, TypeName, Key) {IsTransient = IsTransient};
            Key = newKey; // after old key is saved ! 
            IsTransient = false;
            CacheState();
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