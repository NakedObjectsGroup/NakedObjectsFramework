// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Facade.Impl.Utility;

namespace NakedObjects.Facade.Impl {
    public class ConsentFacade : IConsentFacade {
        private readonly IConsent consent;

        public ConsentFacade(IConsent consent) => this.consent = consent ?? throw new NullReferenceException($"{nameof(consent)} is null");

        #region IConsentFacade Members

        public bool IsAllowed => consent.IsAllowed;

        public bool IsVetoed => consent.IsVetoed;

        public string Reason => consent.Reason;

        public Exception Exception => consent.Exception;

        #endregion

        public override bool Equals(object obj) => obj is ConsentFacade cf && Equals(cf);

        public bool Equals(ConsentFacade other) {
            if (ReferenceEquals(null, other)) { return false; }

            return ReferenceEquals(this, other) || Equals(other.consent, consent);
        }

        public override int GetHashCode() => consent != null ? consent.GetHashCode() : 0;
    }
}