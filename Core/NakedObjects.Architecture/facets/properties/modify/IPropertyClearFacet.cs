// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;

namespace NakedObjects.Architecture.Facets.Properties.Modify {
    /// <summary>
    ///     Mechanism for clearing a property of an object (that is, setting it  to <c>null</c>).
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, typically corresponds to
    ///     a method named <c>ClearXxx</c> (for a property <c>Xxx</c>). As a
    ///     fallback the standard model also supports invoking the <c>set_Xxx</c>
    ///     method with <c>null</c>.
    /// </para>
    public interface IPropertyClearFacet : IFacet {
        void ClearProperty(INakedObject nakedObject, INakedObjectPersistor persistor);
    }
}