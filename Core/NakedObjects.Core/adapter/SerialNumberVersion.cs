// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Globalization;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Adapter {
    public class SerialNumberVersion : AbstractVersion, IEncodedToStrings {
        private readonly long versionNumber;

        public SerialNumberVersion(long number, string user, DateTime? time)
            : base(user, time) {
            versionNumber = number;
        }


        public SerialNumberVersion(string[] strings)
            : base(strings[0], new DateTime(long.Parse(strings[1]))) {
            versionNumber = long.Parse(strings[2]);
        }


        public virtual long Sequence {
            get { return versionNumber; }
        }

        public override string Digest {
            get { return IdentifierUtils.ComputeMD5HashAsString(versionNumber.ToString(CultureInfo.InvariantCulture)); }
        }

        #region IEncodedToStrings Members

        public string[] ToEncodedStrings() {
            var helper = new StringEncoderHelper();

            helper.Add(user);
            helper.Add(time.GetValueOrDefault().Ticks);
            helper.Add(versionNumber);

            return helper.ToArray();
        }

        public string[] ToShortEncodedStrings() {
            return ToEncodedStrings();
        }

        #endregion

        private static long URShift(long number, int bits) {
            return number >= 0 ? number >> bits : (number >> bits) + (2L << ~bits);
        }

        public override int GetHashCode() {
            return (int) (versionNumber ^ (URShift(versionNumber, 32)));
        }

        public override IVersion Next(string userParm, DateTime? timeParm) {
            return new SerialNumberVersion(versionNumber + 1, userParm, timeParm);
        }

        public override bool Equals(IVersion other) {
            if (other is SerialNumberVersion) {
                return versionNumber == ((SerialNumberVersion) other).versionNumber;
            }
            return false;
        }

        public override string AsSequence() {
            return Convert.ToString(versionNumber, 16);
        }

        public override string ToString() {
            return "SerialNumberVersion#" + versionNumber + " " + AsString.Timestamp(time);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}