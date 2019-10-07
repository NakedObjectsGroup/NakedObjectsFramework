// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Adapter {
    public sealed class NullVersion : IVersion, IEncodedToStrings {
        private static readonly ILog Log = LogManager.GetLogger(typeof(NullVersion));

        #region IEncodedToStrings Members

        public string[] ToEncodedStrings() {
            return new string[0];
        }

        public string[] ToShortEncodedStrings() {
            return new string[0];
        }

        #endregion

        #region IVersion Members

        public string User {
            get { return ""; }
        }

        public DateTime? Time {
            get { return DateTime.Now; }
        }

        public string Digest {
            get { return null; }
        }

        public bool IsDifferent(IVersion version) {
            return !Equals(version);
        }

        public bool IsDifferent(string digest) {
            return Digest != digest;
        }

        public string AsSequence() {
            return "";
        }

        public bool Equals(IVersion other) {
            return other is NullVersion;
        }

        #endregion

        public IVersion Next(string user, DateTime time) {
            throw new UnexpectedCallException(Log.LogAndReturn("Unexpected call of 'Next'"));
        }

        public override bool Equals(object other) {
            var a = other as IVersion;
            return a != null && Equals(a);
        }

        public override int GetHashCode() {
            return 0;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}