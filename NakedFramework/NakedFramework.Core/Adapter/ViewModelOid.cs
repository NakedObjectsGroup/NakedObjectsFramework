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
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Adapter {
    public sealed class ViewModelOid : IEncodedToStrings, IViewModelOid {
        private readonly IMetamodelManager metamodel;
        private int cachedHashCode;
        private string cachedToString;
        private ViewModelOid previous;

        public ViewModelOid(IMetamodelManager metamodel, IObjectSpec spec) {
            this.metamodel = metamodel ?? throw new InitialisationException($"{nameof(metamodel)} is null");
            IsTransient = false;
            TypeName = TypeNameUtils.EncodeTypeName(spec.FullName);
            Keys = new[] {Guid.NewGuid().ToString()};
            CacheState();
        }

        public ViewModelOid(IMetamodelManager metamodel, ILoggerFactory loggerFactory, string[] strings) {
            this.metamodel = metamodel ?? throw new InitialisationException($"{nameof(metamodel)} is null");
            var helper = new StringDecoderHelper(metamodel, loggerFactory, loggerFactory.CreateLogger<StringDecoderHelper>(), strings);
            TypeName = helper.GetNextString();

            Keys = helper.HasNext ? helper.GetNextArray() : new[] {Guid.NewGuid().ToString()};

            IsTransient = false;
            CacheState();
        }

        #region IEncodedToStrings Members

        public string[] ToEncodedStrings() {
            var helper = new StringEncoderHelper();
            helper.Add(TypeName);

            if (Keys.Any()) {
                helper.Add(Keys);
            }

            return helper.ToArray();
        }

        public string[] ToShortEncodedStrings() => ToEncodedStrings();

        #endregion

        #region IViewModelOid Members

        public string TypeName { get; }
        public string[] Keys { get; private set; }
        public bool IsFinal { get; private set; }
        public void CopyFrom(IOid oid) { }

        public IOid Previous => previous;

        public bool IsTransient { get; }

        public bool HasPrevious => previous != null;

        public ITypeSpec Spec => metamodel.GetSpecification(TypeNameUtils.DecodeTypeName(TypeName));

        public void UpdateKeys(string[] newKeys, bool final) {
            previous = new ViewModelOid(metamodel, (IObjectSpec) Spec) {Keys = Keys};
            Keys = newKeys; // after old key is saved ! 
            IsFinal = final;
            CacheState();
        }

        #endregion

        private void CacheState() {
            cachedHashCode = HashCodeUtils.Seed;
            cachedHashCode = HashCodeUtils.Hash(cachedHashCode, TypeName);
            cachedHashCode = HashCodeUtils.Hash(cachedHashCode, Keys);

            var keys = Keys.Aggregate((s, t) => $"{s}:{t}");

            cachedToString = $"{(IsTransient ? "T" : "")}VMOID#{keys}{(previous == null ? "" : "+")}";
        }

        #region Object Overrides

        public override bool Equals(object obj) {
            if (obj == this) {
                return true;
            }

            return obj is ViewModelOid oid && TypeName.Equals(oid.TypeName) && Keys.SequenceEqual(oid.Keys);
        }

        // ReSharper disable once NonReadonlyFieldInGetHashCode
        // investigate making Oid immutable
        public override int GetHashCode() => cachedHashCode;

        public override string ToString() => cachedToString;

        #endregion
    }
}