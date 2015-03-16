// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Facet {
    /// <summary>
    ///     Mechanism for obtaining the name of the icon.
    /// </summary>
    /// <para>
    ///     Typically an icon based on the class name is used for every instance of a class (for example, by placing an appropriately
    ///     named image file into a certain directory). This facet allows the icon to be changed for the class or on an
    ///     instance-by-instance basis. For example, the icon might be adapted with an overlay to represent its state
    ///     through some well-defined lifecycle (eg pending approval, approved, rejected). Alternatively a
    ///     <see cref="BoundedAttribute" /> annotated class might have completely different icons for its instances (eg Visa,
    ///     Mastercard, Amex).
    /// </para>
    /// <para>
    ///     In the standard Naked Objects Programming Model, typically corresponds to a method named <c>iconName</c>.
    /// </para>
    /// <seealso cref="ITitleFacet" />
    /// <seealso cref="IPluralFacet" />
    public interface IIconFacet : IFacet {
        /// <summary>
        ///     The name of the icon for <i>this instance</i> of a class.
        /// </summary>
        /// <para>
        ///     In the standard Naked Objects Programming Model, this typically corresponds to a method named <c>iconName</c>.
        /// </para>
        string GetIconName(INakedObjectAdapter nakedObjectAdapter);

        /// <summary>
        ///     The name of the icon for <i>this class</i> of object.
        /// </summary>
        /// <para>
        ///     In the standard Naked Objects Programming Model, this typically corresponds to the <see cref="MaskAttribute" />.
        /// </para>
        string GetIconName();
    }
}