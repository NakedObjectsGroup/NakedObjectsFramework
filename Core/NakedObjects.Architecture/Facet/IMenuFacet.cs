// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Menu;

namespace NakedObjects.Architecture.Facet {
    /// <summary>
    ///     Mechanism for obtaining the action menu for a type.
    /// </summary>
    /// <para>
    ///     
    /// </para>
    /// <para>
    /// In the standard Naked Objects Programming Model, by default the action menu will
    /// be generated from the object's actions, taking account of action ordering.  
    /// However, this may be overridden by writing a method named <c>Menu</c>.
    /// </para>
    public interface IMenuFacet : IFacet {
        /// <summary>
        ///     The Menu for this type.
        /// </summary>
        IMenuImmutable GetMenu();

        void CreateMenu(IMetamodelBuilder metamodel);
    }
}