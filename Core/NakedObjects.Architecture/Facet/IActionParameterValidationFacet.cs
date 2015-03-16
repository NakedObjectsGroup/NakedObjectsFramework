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
    ///     The mechanism by which a single parameter of an action can be validated
    ///     before the action itself is invoked.
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to invoking the
    ///     <c>ValidateNXxx</c> support method for an action.
    /// </para>
    /// <seealso cref="IActionInvocationFacet" />
    public interface IActionParameterValidationFacet : IFacet, IValidatingInteractionAdvisor {
        /// <summary>
        ///     Reason why the validation has failed, or <c>null</c> if okay
        /// </summary>
        string InvalidReason(INakedObjectAdapter target, INakedObjectAdapter paramValue);
    }
}