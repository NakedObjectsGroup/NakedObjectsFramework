// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Facet {
    /// <summary>
    ///     Provides a default value for a property of a newly created object.
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to
    ///     the <c>DefaultXxx</c> supporting method for the property  <c>Xxx</c>.
    /// </para>
    /// <para>
    ///     An alternative mechanism may be to specify the value in the created callback.
    /// </para>
    /// <seealso cref="ICreatedCallbackFacet" />
    public interface IPropertyDefaultFacet : IFacet {
        /// <summary>
        ///     The default value for this property in a newly created object
        /// </summary>
        object GetDefault(INakedObjectAdapter inObjectAdapter);
    }
}