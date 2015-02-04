// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedObjects.Architecture.Interactions {
    /// <summary>
    ///     Mix-in interface for facets that can advise as to whether a member should be disabled.
    /// </summary>
    /// <seealso cref="IValidatingInteractionAdvisor" />
    /// <seealso cref="IHidingInteractionAdvisor" />
    public interface IDisablingInteractionAdvisor {
        /// <summary>
        ///     Whether the rule represented by this facet disables the member to which it applies.
        /// </summary>
        /// <para>
        ///     Implementations should use the provided <see cref="IInteractionContext" /> to
        ///     determine whether they disable the member from being modified or used.  They must
        ///     however guard against a <c>null</c> target <see cref="IInteractionContext.Target" />
        ///     and session <see cref="IInteractionContext.Session" /> - neither are guaranteed to be populated.
        /// </para>
        string Disables(IInteractionContext ic);

        /// <summary>
        ///     Create (not throw) an exception of the appropriate subclass if the validation has failed.
        /// </summary>
        Exception CreateExceptionFor(IInteractionContext ic);
    }
}