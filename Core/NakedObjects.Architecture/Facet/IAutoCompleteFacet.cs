// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Facet {
    /// <summary>
    ///     Provides a set of autocompletions for a property or parameter
    /// </summary>
    /// <para>
    ///     Viewers would typically represent this as a drop-down list box for the property or parameter.
    /// </para>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to
    ///     the <c>AutoCompleteXxx</c> supporting method for the property/parm <c>Xxx</c>.
    /// </para>
    public interface IAutoCompleteFacet : IFacet {
        int MinLength { get; }

        /// <summary>
        ///     Gets the available autocompletions for this property or parm
        /// </summary>
        object[] GetCompletions(INakedObjectAdapter inObjectAdapter, string autoCompleteParm);
    }
}