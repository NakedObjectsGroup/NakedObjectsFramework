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
    ///     The mechanism by which the proposed value of a property can be validated,
    ///     called immediately before <see cref="IPropertySetterFacet" /> setting the value.
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to
    ///     invoking the <c>ValidateXxx</c> method for a property<c>Xxx</c>.
    /// </para>
    /// <seealso cref="IPropertySetterFacet" />
    public interface IPropertyValidateFacet : IFacet, IValidatingInteractionAdvisor {
        /// <summary>
        ///     The reason why the proposed value is invalid.
        /// </summary>
        /// <para>
        ///     Should return <c>null</c> if the value is in fact valid.
        /// </para>
        string InvalidReason(INakedObjectAdapter targetObjectAdapter, INakedObjectAdapter proposedValue);
    }
}