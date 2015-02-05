// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Reflect {
    public interface IConsent {
        /// <summary>
        ///     Returns the permission's reason, detailing why, or why not, permission is being given, or denied
        /// </summary>
        /// <para>
        ///     If an <see cref="Exception" /> is present, then will just equal that exception's
        ///     <see
        ///         cref="System.Exception.Message" />
        /// </para>
        string Reason { get; }

        /// <summary>
        ///     Returns true if this object is giving permission
        /// </summary>
        bool IsAllowed { get; }

        /// <summary>
        ///     Returns true if this object is NOT giving permission
        /// </summary>
        bool IsVetoed { get; }

        /// <summary>
        ///     Represents the reason for an vetoed exception as an inheritance hierarchy
        /// </summary>
        /// <para>
        ///     Consents that are <see cref="IsVetoed" /> will not necessary have a
        ///     <see cref="Reason" /> nor a <see cref="Exception" />, though typically these should
        /// </para>
        /// <para>
        ///     This design allows us to add new checks, (eg for new annotation semantics) without the
        ///     intermediary <see cref="IActionSpec" />s and ActionPeer (etc) needing to be aware of
        ///     these new subtypes
        /// </para>
        Exception Exception { get; }
    }

    // Copyright (c) Naked Objects Group Ltd.
}