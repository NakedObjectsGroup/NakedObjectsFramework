// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Surface.Nof4.Wrapper {
    public class ConsentWrapper : IConsentSurface {
        private readonly IConsent consent;

        public ConsentWrapper(IConsent consent) {
            this.consent = consent;
        }

        #region IConsentSurface Members

        public bool IsAllowed {
            get { return consent.IsAllowed; }
        }

        public bool IsVetoed {
            get { return consent.IsVetoed; }
        }

        public string Reason {
            get { return consent.Reason; }
        }

        public Exception Exception {
            get { return consent.Exception; }
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