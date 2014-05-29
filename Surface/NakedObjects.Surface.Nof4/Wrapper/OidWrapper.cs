// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Surface.Nof4.Wrapper {
    public class OidWrapper : IOidSurface {
        private readonly IOid oid;

        public OidWrapper(IOid oid) {
            this.oid = oid;
        }

        #region IOidSurface Members

        public object Value {
            get { return oid; }
        }

        #endregion

        public override bool Equals(object obj) {
            var oidWrapper = obj as OidWrapper;
            if (oidWrapper != null) {
                return Equals(oidWrapper);
            }
            return false;
        }

        public bool Equals(OidWrapper other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.oid, oid);
        }

        public override int GetHashCode() {
            return (oid != null ? oid.GetHashCode() : 0);
        }
    }
}