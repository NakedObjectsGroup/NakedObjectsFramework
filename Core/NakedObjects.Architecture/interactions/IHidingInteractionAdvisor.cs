// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Component;

namespace NakedObjects.Architecture.Interactions {
    /// <summary>
    ///     Mix-in interface for facets that can advise as to whether a member
    ///     should be hidden.
    /// </summary>
    /// <seealso cref="IDisablingInteractionAdvisor" />
    /// <seealso cref="IValidatingInteractionAdvisor" />
    public interface IHidingInteractionAdvisor {
        /// <summary>
        ///     Whether the rule represented by this facet hides the
        ///     member to which it applies.
        /// </summary>
        /// <para>
        ///     Implementations should use the provided <see cref="IInteractionContext" /> to
        ///     determine whether they disable the member from being modified or used.  They must
        ///     however guard against a <c>null</c> target <see cref="IInteractionContext.Target" />
        ///     and session <see cref="IInteractionContext.Session" /> - neither are guaranteed to be populated.
        /// </para>
        string Hides(IInteractionContext ic, ILifecycleManager lifecycleManager, IMetamodelManager manager);

        /// <summary>
        ///     Create (not throw) an exception to indicate that this
        ///     interaction cannot be performed because the target object or member is hidden.
        /// </summary>
        Exception CreateExceptionFor(IInteractionContext ic, ILifecycleManager lifecycleManager, IMetamodelManager manager);
    }
}