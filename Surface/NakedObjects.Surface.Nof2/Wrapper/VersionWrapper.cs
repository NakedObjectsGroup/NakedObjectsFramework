// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using NakedObjects.Surface;
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