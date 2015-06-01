// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Version = org.nakedobjects.@object.Version;

namespace NakedObjects.Surface.Nof2.Wrapper {
    public class VersionWrapper : IVersionSurface {
        private static readonly MD5CryptoServiceProvider MD5CryptoServiceProvider = new MD5CryptoServiceProvider();
        private readonly Version version;

        public VersionWrapper(Version version) {
            this.version = version;
        }

        public DateTime? Time {
            get {
                if (version != null) {
                    var javaEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    long ticksToEpoch = javaEpoch.Ticks;
                    long ticksFromEpoch = version.getTime().getTime()*10000;
                    return new DateTime(ticksToEpoch + ticksFromEpoch);
                }
                return DateTime.UtcNow;
            }
        }

        #region IVersionSurface Members

        public string Digest {
            get {
                // todo need to move this code into versions and add Digest to IVersion interface
                // for the moment just use time 

                return Time != null ? ComputeMD5HashAsString(Time.Value.Ticks.ToString(CultureInfo.InvariantCulture)) : null;
            }
        }

        public bool IsDifferent(string digest) {
            return Digest != digest;
        }

        #endregion

        public static string ComputeMD5HashAsString(string s) {
            return (Math.Abs(BitConverter.ToInt64(ComputMD5HashFromString(s), 0)).ToString(CultureInfo.InvariantCulture));
        }

        private static byte[] ComputMD5HashFromString(string s) {
            byte[] idAsBytes = Encoding.UTF8.GetBytes(s);
            return MD5CryptoServiceProvider.ComputeHash(idAsBytes);
        }
    }
}