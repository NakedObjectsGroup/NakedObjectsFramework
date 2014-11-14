// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Adapter {
    public class FileVersion : AbstractVersion, IEncodedToStrings {
        private static IClock clock;

        public FileVersion(string user)
            : this(user, clock.Ticks) {}

        public FileVersion(string user, long sequence)
            : base(user, new DateTime(sequence)) {}

        public FileVersion(string[] strings)
            : base(strings[0], new DateTime(long.Parse(strings[1]))) {}

        public static IClock Clock {
            set { clock = value; }
        }

        public virtual long Sequence {
            get { return time.GetValueOrDefault().Ticks; }
        }

        #region IEncodedToStrings Members

        public string[] ToEncodedStrings() {
            var helper = new StringEncoderHelper();

            helper.Add(user);
            helper.Add(time.GetValueOrDefault().Ticks);

            return helper.ToArray();
        }

        public string[] ToShortEncodedStrings() {
            return ToEncodedStrings();
        }

        #endregion

        public override bool Equals(IVersion other) {
            if (other is FileVersion) {
                return IsSameTime((FileVersion) other);
            }
            return false;
        }

        public override IVersion Next(string newUser, DateTime? newTime) {
            throw new NotImplementedException();
        }

        private bool IsSameTime(FileVersion other) {
            return time.GetValueOrDefault().Ticks == other.time.GetValueOrDefault().Ticks;
        }

        public override bool Equals(object obj) {
            if (obj is IVersion) {
                return Equals((IVersion) obj);
            }
            return false;
        }

        public override int GetHashCode() {
            return time.GetValueOrDefault().Ticks.GetHashCode();
        }

        public override string AsSequence() {
            return Convert.ToString(Sequence, 16);
        }

        public override string ToString() {
            var str = new AsString(this);
            str.Append("sequence", time.GetValueOrDefault().Ticks);
            str.Append("time", time);
            str.Append("user", user);
            return str.ToString();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}