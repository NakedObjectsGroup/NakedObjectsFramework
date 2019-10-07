// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Core.Reflect {
    public abstract class ConsentAbstract : IConsent {
        private readonly string reason;

        protected internal ConsentAbstract() {
            Exception = null;
            reason = null;
        }

        protected internal ConsentAbstract(string reason) {
            Exception = null;
            this.reason = reason;
        }

        protected internal ConsentAbstract(Exception exception) {
            this.Exception = exception;
            reason = exception != null ? exception.Message : null;
        }

        #region IConsent Members

        /// <summary>
        ///     Returns the permission's reason
        /// </summary>
        public virtual string Reason {
            get { return reason ?? ""; }
        }

        public virtual Exception Exception { get; }

        /// <summary>
        ///     Returns true if this object is giving permission
        /// </summary>
        public abstract bool IsAllowed { get; }

        /// <summary>
        ///     Returns <c>true</c> if this object is NOT giving permission
        /// </summary>
        public abstract bool IsVetoed { get; }

        #endregion

        /// <summary>
        ///     Returns an Allow (Allow.Default) object if true; Veto (Veto.Default) if false
        /// </summary>
        public static IConsent GetAllow(bool allow) {
            return allow ? (IConsent) Allow.Default : Veto.Default;
        }

        /// <summary>
        ///     Returns a new Allow object if <c>allow</c> is <c>true</c>; a new Veto if <c>false</c>. The respective reason
        ///     is passed to the newly created object.
        /// </summary>
        public static IConsent Create(bool allow, string reasonAllowed, string reasonVeteod) {
            return allow ? (IConsent) new Allow(reasonAllowed) : new Veto(reasonVeteod);
        }

        public static IConsent Create(string vetoReason) {
            return vetoReason == null ? (IConsent) Allow.Default : new Veto(vetoReason);
        }

        public override string ToString() {
            return "Permission [type=" + (IsVetoed ? "VETOED" : "ALLOWED") + ", reason=" + reason + "]";
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}