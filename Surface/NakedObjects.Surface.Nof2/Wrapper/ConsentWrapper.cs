// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects.Surface;
using org.nakedobjects.@object.control;

namespace NakedObjects.Surface.Nof2.Wrapper {
    public class ConsentWrapper : IConsentSurface {
        private readonly Consent consent;

        public ConsentWrapper(Consent consent) {
            this.consent = consent;
        }

        #region IConsentSurface Members

        public bool IsAllowed {
            get { return consent.isAllowed(); }
        }

        public bool IsVetoed {
            get { return consent.isVetoed(); }
        }

        public string Reason {
            get { return consent.getReason(); }
        }

        public Exception Exception {
            get { return null; }
        }

        #endregion

        public override bool Equals(object obj) {
            var consentWrapper = obj as ConsentWrapper;
            if (consentWrapper != null) {
                return Equals(consentWrapper);
            }
            return false;
        }

        public bool Equals(ConsentWrapper other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.consent, consent);
        }

        public override int GetHashCode() {
            return (consent != null ? consent.GetHashCode() : 0);
        }
    }
}