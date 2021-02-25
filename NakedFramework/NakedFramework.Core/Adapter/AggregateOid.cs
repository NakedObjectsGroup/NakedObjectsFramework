// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Exception;

namespace NakedFramework.Core.Adapter {
    public sealed class AggregateOid : IEncodedToStrings, IAggregateOid {
        private readonly IMetamodelManager metamodel;
        private readonly string typeName;

        public AggregateOid(IMetamodelManager metamodel, IOid oid, string id, string typeName) {
            this.metamodel = metamodel ?? throw new InitialisationException($"{nameof(metamodel)} is null");
            ParentOid = oid ?? throw new InitialisationException($"{nameof(oid)} is null");
            FieldName = id ?? throw new InitialisationException($"{nameof(id)} is null");
            this.typeName = typeName ?? throw new InitialisationException($"{nameof(typeName)} is null");
        }

        public AggregateOid(IMetamodelManager metamodel, ILoggerFactory loggerFactory, string[] strings) {
            this.metamodel = metamodel ?? throw new InitialisationException($"{nameof(metamodel)} is null");
            var helper = new StringDecoderHelper(metamodel, loggerFactory, loggerFactory.CreateLogger<StringDecoderHelper>(), strings);
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

        public void CopyFrom(IOid oid) => throw new NakedObjectSystemException("CopyFrom not supported on Aggregate oid");

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