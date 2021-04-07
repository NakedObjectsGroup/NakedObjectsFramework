// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Adapter;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;

namespace NakedFramework.Core.Persist {
    public sealed class DatabaseOid : IEncodedToStrings, IDatabaseOid {
        private readonly ILogger<DatabaseOid> logger;
        private readonly IMetamodelManager metamodel;
        private int cachedHashCode;
        private string cachedToString;
        private DatabaseOid previous;

        private void CacheState() {
            cachedHashCode = HashCodeUtils.Seed;
            cachedHashCode = HashCodeUtils.Hash(cachedHashCode, TypeName);
            cachedHashCode = HashCodeUtils.Hash(cachedHashCode, Key);

            var keys = Key.Aggregate((s, t) => $"{s}:{t}");

            cachedToString = $"{(IsTransient ? "T" : "")}EOID#{keys}{(previous == null ? "" : "+")}";
        }

        private void ThrowErrorIfNotTransient(object[] newKey = null) {
            if (!IsTransient) {
                var newKeyString = newKey?.Aggregate("New Key", (s, t) => $"{s} : {t}") ?? "";
                var error = $"Attempting to make persistent an already persisted object. Type {TypeName}  Existing Key: {cachedToString} {newKeyString}";
                throw new NotPersistableException(logger.LogAndReturn(error));
            }
        }

        #region IEncodedToStrings Members

        public string[] ToEncodedStrings() {
            var helper = new StringEncoderHelper();

            helper.Add(TypeName);
            helper.Add(Key);
            helper.Add(IsTransient);
            helper.Add(HasPrevious);
            if (HasPrevious) {
                helper.Add(previous);
            }

            return helper.ToArray();
        }

        public string[] ToShortEncodedStrings() {
            var helper = new StringEncoderHelper();

            helper.Add(TypeName);
            helper.Add(Key);
            helper.Add(IsTransient);

            return helper.ToArray();
        }

        #endregion

        #region IDatabaseOid Members

        public string TypeName { get; private set; }
        public object[] Key { get; private set; }

        public void CopyFrom(IOid oid) {
            var from = oid as DatabaseOid ?? throw new NakedObjectSystemException("Copy from Oid must be Entity Oid");
            Key = from.Key;
            TypeName = from.TypeName;
            IsTransient = from.IsTransient;
            CacheState();
        }

        public ITypeSpec Spec => metamodel.GetSpecification(TypeNameUtils.DecodeTypeName(TypeName));

        public IOid Previous => previous;

        public bool HasPrevious => previous != null;

        public bool IsTransient { get; private set; }

        public void MakePersistent() {
            ThrowErrorIfNotTransient();
            previous = new DatabaseOid(metamodel, TypeName, Key, logger) {IsTransient = IsTransient};
            IsTransient = false;
            CacheState();
        }

        public void MakePersistentAndUpdateKey(object[] newKey) {
            ThrowErrorIfNotTransient(newKey);
            UpdateKey(newKey);
        }

        public void UpdateKey(object[] newKey)
        {
            previous = new DatabaseOid(metamodel, TypeName, Key, logger) { IsTransient = IsTransient };
            Key = newKey; // after old key is saved ! 
            IsTransient = false;
            CacheState();
        }


        #endregion

        #region Constructors

        public DatabaseOid(IMetamodelManager metamodel, Type type, object[] key, bool isTransient, ILogger<DatabaseOid> logger)
            : this(metamodel, type.FullName, key, logger) {
            IsTransient = isTransient;
            CacheState();
        }

        public DatabaseOid(IMetamodelManager metamodel, string typeName, object[] key, ILogger<DatabaseOid> logger) {
            this.metamodel = metamodel ?? throw new InitialisationException($"{nameof(metamodel)} is null");
            this.logger = logger ?? throw new InitialisationException($"{nameof(logger)} is null");
            TypeName = TypeNameUtils.EncodeTypeName(typeName);
            Key = key;
            IsTransient = false;
            CacheState();
        }

        public DatabaseOid(IMetamodelManager metamodel, ILoggerFactory loggerFactory, string[] strings) {
            this.metamodel = metamodel ?? throw new InitialisationException($"{nameof(metamodel)} is null");
            logger = loggerFactory.CreateLogger<DatabaseOid>();
            var helper = new StringDecoderHelper(metamodel, loggerFactory, loggerFactory.CreateLogger<StringDecoderHelper>(), strings);

            TypeName = helper.GetNextString();
            Key = helper.GetNextObjectArray();
            IsTransient = helper.GetNextBool();

            if (helper.HasNext) {
                var hasPrevious = helper.GetNextBool();
                if (hasPrevious) {
                    previous = (DatabaseOid) helper.GetNextEncodedToStrings();
                }
            }

            CacheState();
        }

        #endregion

        #region Object Overrides

        public override bool Equals(object obj) =>
            obj == this ||
            obj is DatabaseOid oid &&
            TypeName.Equals(oid.TypeName) &&
            Key.SequenceEqual(oid.Key);

        // ReSharper disable once NonReadonlyFieldInGetHashCode
        public override int GetHashCode() => cachedHashCode;

        public override string ToString() => cachedToString;

        #endregion
    }
}