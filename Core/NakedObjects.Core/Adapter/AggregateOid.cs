// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Diagnostics;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Adapter {
    public sealed class AggregateOid : IEncodedToStrings, IAggregateOid {
        private readonly IMetamodelManager metamodel;
        private readonly string typeName;

        public AggregateOid(IMetamodelManager metamodel, IOid oid, string id, string typeName) {
            Assert.AssertNotNull(metamodel);
            Assert.AssertNotNull(oid);
            Assert.AssertNotNull(id);
            Assert.AssertNotNull(typeName);

            this.metamodel = metamodel;
            ParentOid = oid;
            FieldName = id;
            this.typeName = typeName;
        }

        public AggregateOid(IMetamodelManager metamodel, string[] strings) {
            Assert.AssertNotNull(metamodel);
            this.metamodel = metamodel;
            var helper = new StringDecoderHelper(metamodel, strings);
            typeName = helper.GetNextString();
            FieldName = helper.GetNextString();
            if (helper.HasNext) {
                ParentOid = (IOid) helper.GetNextEncodedToStrings();
            }
        }

        #region IAggregateOid Members

        public IOid ParentOid { get; }

        public string FieldName { get; }

        public IOid Previous => null;

        public bool IsTransient => ParentOid.IsTransient;

        public void CopyFrom(IOid oid) => Trace.Assert(false, "CopyFRom not supported on Aggregate oid");

        public ITypeSpec Spec => metamodel.GetSpecification(typeName);

        public bool HasPrevious => false;

        #endregion

        #region IEncodedToStrings Members

        public string[] ToEncodedStrings() {
            var helper = new StringEncoderHelper();
            helper.Add(typeName);
            helper.Add(FieldName);
            if (ParentOid != null) {
                helper.Add(ParentOid as IEncodedToStrings);
            }

            return helper.ToArray();
        }

        public string[] ToShortEncodedStrings() => ToEncodedStrings();

        #endregion

        public override bool Equals(object obj) {
            if (obj == this) {
                return true;
            }

            var otherOid = obj as AggregateOid;
            return otherOid != null && Equals(otherOid);
        }

        private bool Equals(AggregateOid otherOid) =>
            otherOid.ParentOid.Equals(ParentOid) &&
            otherOid.FieldName.Equals(FieldName) &&
            otherOid.typeName.Equals(typeName);

        public override int GetHashCode() {
            var hashCode = 17;
            hashCode = 37 * hashCode + ParentOid.GetHashCode();
            hashCode = 37 * hashCode + FieldName.GetHashCode();
            return hashCode;
        }

        public override string ToString() => "AOID[" + ParentOid + "," + FieldName + "]";
    }

    // Copyright (c) Naked Objects Group Ltd.
}