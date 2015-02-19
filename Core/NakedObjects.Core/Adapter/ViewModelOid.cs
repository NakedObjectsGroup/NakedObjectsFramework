// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Adapter {
    internal class ViewModelOid : IEncodedToStrings, IViewModelOid {
        private readonly IMetamodelManager metamodel;
        private readonly int cachedHashCode;
        private readonly string cachedToString;
        private readonly ViewModelOid previous;

        private ViewModelOid(string[] keys, ViewModelOid previous) {
            Assert.AssertNotNull(metamodel);
            this.metamodel = previous.metamodel;
            IsTransient = false;
            TypeName = previous.TypeName;
            Keys = keys;
            this.previous = previous;
            var cache = CacheState();
            cachedHashCode = cache.Item1;
            cachedToString = cache.Item2;
        }

        public ViewModelOid(IMetamodelManager metamodel, IObjectSpec spec) {
            Assert.AssertNotNull(metamodel);
            this.metamodel = metamodel;
            IsTransient = false;
            TypeName = TypeNameUtils.EncodeTypeName(spec.FullName);
            Keys = new[] {Guid.NewGuid().ToString()};
            var cache = CacheState();
            cachedHashCode = cache.Item1;
            cachedToString = cache.Item2;
        }

        public ViewModelOid(IMetamodelManager metamodel, string[] strings) {
            Assert.AssertNotNull(metamodel);
            this.metamodel = metamodel;
            var helper = new StringDecoderHelper(metamodel, strings);
            TypeName = helper.GetNextString();

            Keys = helper.HasNext ? helper.GetNextArray() : new[] {Guid.NewGuid().ToString()};

            IsTransient = false;
            var cache = CacheState();
            cachedHashCode = cache.Item1;
            cachedToString = cache.Item2;
        }

        public string TypeName { get; private set; }
        public string[] Keys { get; private set; }
        public bool IsFinal { get; private set; }

        #region IEncodedToStrings Members

        public string[] ToEncodedStrings() {
            var helper = new StringEncoderHelper();
            helper.Add(TypeName);

            if (Keys.Any()) {
                helper.Add(Keys);
            }

            return helper.ToArray();
        }

        public string[] ToShortEncodedStrings() {
            return ToEncodedStrings();
        }

        #endregion

        #region IOid Members

        public IOid CopyFrom(IOid oid) {
            return oid;
        }

        public IOid Previous {
            get { return previous; }
        }

        public bool IsTransient { get; private set; }

        public bool HasPrevious {
            get { return previous != null; }
        }

        public ITypeSpec Spec {
            get { return metamodel.GetSpecification(TypeNameUtils.DecodeTypeName(TypeName)); }
        }

        #endregion

        private Tuple<int, string> CacheState() {
            var hashCode = HashCodeUtils.Seed;
            hashCode = HashCodeUtils.Hash(hashCode, TypeName);
            hashCode = HashCodeUtils.Hash(hashCode, Keys);

            object keys = Keys.Aggregate((s, t) => s + ":" + t);

            var toString = string.Format("{0}VMOID#{1}{2}", IsTransient ? "T" : "", keys, previous == null ? "" : "+");
            return new Tuple<int, string>(hashCode, toString);
        }

        public IOid UpdateKeys(string[] newKeys, bool final) {
            return new ViewModelOid(newKeys, this) {IsFinal = final};
        }

        #region Object Overrides

        public override bool Equals(object obj) {
            if (obj == this) {
                return true;
            }
            var oid = obj as ViewModelOid;
            if (oid != null) {
                return TypeName.Equals(oid.TypeName) && Keys.SequenceEqual(oid.Keys);
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