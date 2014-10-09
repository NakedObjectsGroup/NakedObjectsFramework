// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Persist {
    public sealed class SerialOid : IOid, IEncodedToStrings {
        private readonly IMetamodelManager metamodel;
        private const bool Persistent = false;
        private const bool Transient = true;
        private readonly string typeName;

        private string cachedStateString;

        private int hashCode;
        private bool isTransient;
        private SerialOid previous;
        private long serialNo;

        private SerialOid(IMetamodelManager metamodel, long serialNo, string typeName, bool isTransient) {
            Assert.AssertNotNull(metamodel);
            this.metamodel = metamodel;
            this.serialNo = serialNo;
            this.typeName = TypeNameUtils.EncodeTypeName(typeName);
            this.isTransient = isTransient;
            CacheState();
        }


        public SerialOid(IMetamodelManager metamodel, string[] strings) {
            Assert.AssertNotNull(metamodel);

            this.metamodel = metamodel;
            var helper = new StringDecoderHelper(metamodel, strings);

            typeName = helper.GetNextString();
            serialNo = helper.GetNextLong();
            isTransient = helper.GetNextBool();

            if (helper.HasNext) {
                bool hasPrevious = helper.GetNextBool();
                if (hasPrevious) {
                    previous = (SerialOid) helper.GetNextEncodedToStrings();
                }
            }
            CacheState();
        }


        public long SerialNo {
            get { return serialNo; }
        }

        #region IEncodedToStrings Members

        public string[] ToEncodedStrings() {
            var helper = new StringEncoderHelper();

            helper.Add(typeName);
            helper.Add(serialNo);
            helper.Add(isTransient);
            helper.Add(previous != null);

            if (previous != null) {
                helper.Add(previous as IEncodedToStrings);
            }

            return helper.ToArray();
        }

        public string[] ToShortEncodedStrings() {
            var helper = new StringEncoderHelper();

            helper.Add(typeName);
            helper.Add(serialNo);
            helper.Add(isTransient);
            return helper.ToArray();
        }

        #endregion

        #region IOid Members

        public IOid Previous {
            get { return previous; }
        }

        public bool IsTransient {
            get { return isTransient; }
        }

        public void CopyFrom(IOid oid) {
            Assert.AssertTrue("Copy from oid must be a serial oid", oid is SerialOid);
            var from = (SerialOid) oid;
            serialNo = from.serialNo;
            isTransient = from.isTransient;
            CacheState();
        }

        public INakedObjectSpecification Specification {
            get { return metamodel.GetSpecification(TypeNameUtils.DecodeTypeName(typeName)); }
        }

        public bool HasPrevious {
            get { return previous != null; }
        }

        #endregion

        public static SerialOid CreatePersistent(IMetamodelManager reflector, long serialNo, string typeName) {
            return new SerialOid(reflector, serialNo, typeName, Persistent);
        }

        public static SerialOid CreateTransient(IMetamodelManager reflector, long serialNo, string typeName) {
            return new SerialOid(reflector, serialNo, typeName, Transient);
        }

        private static long UrShift(long number, int bits) {
            if (number >= 0) {
                return number >> bits;
            }
            return (number >> bits) + (2L << ~bits);
        }

        private void CacheState() {
            hashCode = 17;
            hashCode = 37*hashCode + (int) (serialNo ^ (UrShift(serialNo, 32)));
            hashCode = 37*hashCode + (isTransient ? 0 : 1);
            cachedStateString = (isTransient ? "T" : "") + "OID#" + Convert.ToString(serialNo, 16).ToUpper() + (previous == null ? "" : "+");
        }

        internal void MakePersistent(long newSerialNo) {
            Assert.AssertTrue("Attempting to make persistent a non transient oid", isTransient);
            previous = new SerialOid(metamodel, serialNo, typeName, isTransient);
            serialNo = newSerialNo;
            isTransient = false;
            CacheState();
        }

        public override bool Equals(object obj) {
            var otherOid = obj as SerialOid;
            return otherOid != null && Equals(otherOid);
        }


        // Overloaded to allow compiler to link directly if we know the compile-time type. 
        // TODO (possible performance improvement - called 166,000 times in normal ref data fixture

        public bool Equals(SerialOid otherOid) {
            if (otherOid == this) {
                return true;
            }
            return otherOid.serialNo == serialNo &&
                   otherOid.isTransient == isTransient;
        }

        public override int GetHashCode() {
            return hashCode;
        }

        public override string ToString() {
            return cachedStateString;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}