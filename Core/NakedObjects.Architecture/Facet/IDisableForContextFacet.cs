// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facet {
    /// <summary>
    ///     Disable a property, collection or action based on the state of the target <see cref="INakedObjectAdapter" /> object.
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to
    ///     invoking the <c>DisableXxx</c> support method for the member.
    /// </para>
    public interface IDisableForContextFacet : IFacet, IDisablingInteractionAdvisor {
        /// <summary>
        ///     The reason this object is disabled, or <c>null</c> otherwise
        /// </summary>
        string DisabledReason(INakedObjectAdapter nakedObjectAdapter);
    }

    // Copyright (c) Naked Objects Group Ltd.
}