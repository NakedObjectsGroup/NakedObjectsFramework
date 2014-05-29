// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Surface.Nof4.Wrapper {
    public class VersionWrapper : IVersionSurface {
        private readonly IVersion version;


        public VersionWrapper(IVersion version) {
            this.version = version;
        }

        public DateTime? Time {
            get { return version.Time; }
        }

        #region IVersionSurface Members

        public string Digest {
            get { return version.Digest; }
        }

        public bool IsDifferent(string digest) {
            return version.IsDifferent(digest);
        }

        #endregion
    }
}