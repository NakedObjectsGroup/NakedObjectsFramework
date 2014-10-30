// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

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