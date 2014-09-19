// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedObjects.Architecture.Facets.Ordering.MemberOrder {
    /// <summary>
    ///     The preferred mechanism for determining the order in which the members of the object should
    ///     be rendered
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to annotating each of the
    ///     member methods with the <see cref="MemberOrderAttribute" />.  An alternative approach is to use the
    ///     action <see cref="IActionOrderFacet" /> or field <see cref="IFieldOrderFacet" /> order facets
    /// </para>
    /// <seealso cref="IMemberOrderFacet" />
    /// <seealso cref="IFieldOrderFacet" />
    public interface IMemberOrderFacet : IMultipleValueFacet {
        /// <summary>
        ///     To group members
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The sequence, in dewey-decimal notation
        /// </summary>
        string Sequence { get; }
    }
}