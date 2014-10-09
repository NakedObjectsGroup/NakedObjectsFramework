// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Persist {
    public class AggregateOid : IOid, IEncodedToStrings {
        private readonly string fieldName;
        private readonly IMetamodel metamodel;
        private readonly IOid parentOid;
        private readonly string typeName;

        public AggregateOid(IMetamodel metamodel, IOid oid, string id, string typeName) {
            Assert.AssertNotNull(metamodel);

            this.metamodel = metamodel;
            parentOid = oid;
            fieldName = id;
            this.typeName = typeName;
        }

        public AggregateOid(IMetamodel metamodel, string[] strings) {
            Assert.AssertNotNull(metamodel);
            this.metamodel = metamodel;
            var helper = new StringDecoderHelper(metamodel, strings);
            typeName = helper.GetNextString();
            fieldName = helper.GetNextString();
            if (helper.HasNext) {
                parentOid = (IOid) helper.GetNextEncodedToStrings();
            }
        }

        public virtual IOid ParentOid {
            get { return parentOid; }
        }

        public virtual string FieldName {
            get { return fieldName; }
        }

        #region IOid Members

        public string[] ToEncodedStrings() {
            var helper = new StringEncoderHelper();
            helper.Add(typeName);
            helper.Add(fieldName);
            if (parentOid != null) {
                helper.Add(parentOid as IEncodedToStrings);
            }
            return helper.ToArray();
        }

        public string[] ToShortEncodedStrings() {
            return ToEncodedStrings();
        }

        public virtual IOid Previous {
            get { return null; }
        }

        public virtual bool IsTransient {
            get { return parentOid.IsTransient; }
        }

        public virtual void CopyFrom(IOid oid) {
            throw new NotImplementedException();
        }

        public INakedObjectSpecification Specification {
            get { return metamodel.GetSpecification(typeName); }
        }

        public virtual bool HasPrevious {
            get { return false; }
        }

        #endregion

        public override bool Equals(object obj) {
            if (obj == this) {
                return true;
            }
            var otherOid = obj as AggregateOid;
            return otherOid != null && Equals(otherOid);
        }

        private bool Equals(AggregateOid otherOid) {
            return otherOid.parentOid.Equals(parentOid) &&
                   otherOid.fieldName.Equals(fieldName) &&
                   otherOid.typeName.Equals(typeName);
        }

        public override int GetHashCode() {
            int hashCode = 17;
            hashCode = 37*hashCode + parentOid.GetHashCode();
            hashCode = 37*hashCode + fieldName.GetHashCode();
            return hashCode;
        }

        public override string ToString() {
            return "AOID[" + parentOid + "," + fieldName + "]";
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}