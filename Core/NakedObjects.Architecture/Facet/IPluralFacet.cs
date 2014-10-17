// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedObjects.Architecture.Facet {
    /// <summary>
    ///     Mechanism for obtaining the plural title of an instance of a class, used to label a collection of
    ///     a certain class
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, typically corresponds to a method named <c>PluralName</c>.
    ///     If no plural name is provided, then the framework will attempt to guess the plural name (by adding an
    ///     <i>s</i> or <i>ies</i> suffix).
    /// </para>
    /// <seealso cref="IIconFacet" />
    /// <seealso cref="ITitleFacet" />
    public interface IPluralFacet : ISingleStringValueFacet {}
}