// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

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