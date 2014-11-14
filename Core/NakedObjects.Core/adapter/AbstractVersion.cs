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
    public abstract class AbstractVersion : IVersion {
        protected internal readonly DateTime? time;
        protected internal readonly string user;

        protected AbstractVersion(string user, DateTime? time) {
            this.user = user;
            this.time = time;
        }

        #region IVersion Members

        public virtual DateTime? Time {
            get { return time; }
        }

        public virtual string Digest {
            get { return Time != null ? IdentifierUtils.ComputeMD5HashAsString(Time.Value.Ticks.ToString(CultureInfo.InvariantCulture)) : null; }
        }

        public virtual string User {
            get { return user; }
        }

        public bool IsDifferent(IVersion version) {
            return !Equals(version);
        }

        public bool IsDifferent(string digest) {
            return Digest != digest;
        }

        public abstract string AsSequence();
        public abstract bool Equals(IVersion other);

        #endregion

        public abstract IVersion Next(string newUser, DateTime? newTime);

        //protected internal abstract AbstractVersion Next();

        public override bool Equals(object obj) {
            if (obj is IVersion) {
                return Equals((IVersion) obj);
            }
            return false;
        }

        public abstract override int GetHashCode();
    }

    // Copyright (c) Naked Objects Group Ltd.
}