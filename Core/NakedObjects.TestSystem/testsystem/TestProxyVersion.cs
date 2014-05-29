// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Globalization;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;

namespace NakedObjects.TestSystem {
    public class TestProxyVersion : IVersion {
        private readonly int value;

        public TestProxyVersion()
            : this(1) {}

        public TestProxyVersion(int value) {
            this.value = value;
        }

        #region IVersion Members

        public string Digest {
            get { return Time != null ? IdentifierUtils.ComputeMD5HashAsString(Time.Value.Ticks.ToString(CultureInfo.InvariantCulture)) : null; }
        }

        public bool IsDifferent(IVersion version) {          
            return !Equals(version); 
        }

        public bool IsDifferent(string digest) {
            return Digest != digest;
        }

        public string User {
            get { return "USER"; }
        }

        public DateTime? Time {
            get { return new DateTime(); }
        }

        public string AsSequence() {
            return "" + value;
        }

        #endregion

        public bool Equals(IVersion other) {
            if (other is TestProxyVersion) {
                return ((TestProxyVersion)other).value == value;
            }
            return false;
        }

        public override bool Equals(object obj) {
            if (obj is IVersion) {
                return Equals((IVersion)obj);
            }
            return false;
        }

        public override string ToString() {
            return "IVersion#" + value;
        }

        public IVersion Next() {
            return new TestProxyVersion(value + 1);
        }

        public override int GetHashCode() {
            return value.GetHashCode();
        }
    }
}