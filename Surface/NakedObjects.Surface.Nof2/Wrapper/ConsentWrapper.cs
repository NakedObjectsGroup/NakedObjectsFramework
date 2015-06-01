// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
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
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return Equals(other.consent, consent);
        }

        public override int GetHashCode() {
            return (consent != null ? consent.GetHashCode() : 0);
        }
    }
}