// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.TestSystem {
    public class TestProxyOid : IOid {
        private int id;
        public IOid previous;
        public bool transient = true;

        public TestProxyOid(int id) {
            this.id = id;
        }

        public TestProxyOid(int id, bool persistent) {
            this.id = id;
            transient = !persistent;
        }

        #region IOid Members

        public bool HasPrevious {
            get { return previous != null; }
        }

        public IOid Previous {
            get { return previous; }
        }

        public void CopyFrom(IOid oid) {
            id = ((TestProxyOid) oid).id;
            transient = ((TestProxyOid) oid).transient;
        }

        public INakedObjectSpecification Specification {
            get { throw new NotImplementedException(); }
        }

        public bool IsTransient {
            get { return transient; }
        }

        #endregion

        public override bool Equals(object obj) {
            if (obj == this) {
                return true;
            }
            if (obj is TestProxyOid) {
                return ((TestProxyOid) obj).id == id && ((TestProxyOid) obj).transient == transient;
            }
            return false;
        }

        public override int GetHashCode() {
            return 37*17 + (id ^ (id >> 32)) + (transient ? 0 : 1);
        }

        public override string ToString() {
            return "IOid#" + id + (transient ? " T" : "") + (HasPrevious ? " (" + previous + ")" : "");
        }

        public void MakePersistent(int id) {
            this.id = id;
            transient = false;
        }

        public void SetupPrevious(IOid previous) {
            this.previous = previous;
        }
    }
}