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
    ///     Whether a property or a parameter is mandatory or optional.
    /// </summary>
    /// <para>
    ///     For a mandatory property, the object cannot be saved/updated without
    ///     the value being provided.  For a mandatory parameter, the action cannot
    ///     be invoked without the value being provided.
    /// </para>
    /// <para>
    ///     In the standard Naked Objects Programming Model, a property or parameter is mandatory
    ///     <i>unless</i> it is annotated with the <see cref="OptionallyAttribute" /> annotation.
    /// </para>
    public interface IMandatoryFacet : IMarkerFacet, IValidatingInteractionAdvisor {
        /// <summary>
        ///     Indicates that it is mandatory, that it is <i>not</i> optional.
        /// </summary>
        bool IsMandatory { get; }

        /// <summary>
        ///     Indicates that it is optional, that it is <i>not</i> mandatory.
        /// </summary>
        bool IsOptional { get; }

        /// <summary>
        ///     Whether this value is required but has not been provided (and is therefore invalid).
        /// </summary>
        /// <para>
        ///     If the value has been provided, <i>or</i> if the property or parameter is not required,
        ///     then will return <c>false</c>.
        /// </para>
        bool IsRequiredButNull(INakedObjectAdapter nakedObjectAdapter);
    }
}