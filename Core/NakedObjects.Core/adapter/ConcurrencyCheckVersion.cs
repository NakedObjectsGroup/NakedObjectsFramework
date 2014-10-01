// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Util;

namespace NakedObjects.EntityObjectStore {
    public class ConcurrencyCheckVersion : IVersion, IEncodedToStrings {
        private readonly DateTime time;
        private readonly string user;
        private readonly object version;

        public ConcurrencyCheckVersion(string user, DateTime time, object version) {
            this.user = user;
            this.time = time;
            this.version = version;
        }

        public ConcurrencyCheckVersion(IMetadata metadata, string[] strings) {
            Assert.AssertNotNull(metadata);
            var helper = new StringDecoderHelper(metadata, strings);

            user = helper.GetNextString();
            time = new DateTime(helper.GetNextLong());
            version = helper.GetNextObject();
        }

        #region IEncodedToStrings Members

        public string[] ToEncodedStrings() {
            var helper = new StringEncoderHelper();

            helper.Add(user);
            helper.Add(time.Ticks);
            helper.Add(version);

            return helper.ToArray();
        }

        #endregion

        #region IVersion Members

        public string User {
            get { return user; }
        }

        public DateTime? Time {
            get { return time; }
        }

        public string Digest {
            get { return version != null ? IdentifierUtils.ComputeMD5HashAsString(version.ToString()) : null; }
        }

        public bool IsDifferent(IVersion otherVersion) {
            return !Equals(otherVersion);
        }

        public bool IsDifferent(string digest) {
            return Digest != digest;
        }

        public string AsSequence() {
            return version.ToString();
        }

        #endregion

        public bool Equals(IVersion other) {
            var entityVersion = other as ConcurrencyCheckVersion;
            return entityVersion != null && version.Equals(entityVersion.version);
        }

        #region IEncodedToStrings Members

        public string[] ToShortEncodedStrings() {
            throw new NotImplementedException();
        }

        #endregion

        public override bool Equals(object obj) {
            var entityVersion = obj as ConcurrencyCheckVersion;
            return Equals(entityVersion);
        }

        public override int GetHashCode() {
            return version.GetHashCode();
        }

        public override string ToString() {
            return string.Format("Version: {0} (last read at : {1} by : {2})", version, Time, User);
        }
    }
}