// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;

namespace NakedObjects.Architecture.Facet {
    /// <summary>
    ///     Indicates that this class can parse an entry string
    /// </summary>
    public interface IParseableFacet : IFacet {
        /// <summary>
        ///     Parses a text entry made by a user and sets the domain object's value.
        /// </summary>
        INakedObjectAdapter ParseTextEntry(string text, INakedObjectManager manager);

        /// <summary>
        ///     Parses an invariant value and sets the domain objects value
        /// </summary>
        INakedObjectAdapter ParseInvariant(string text, INakedObjectManager manager);

        /// <summary>
        ///     A title for the object that is valid but which may be easier to
        ///     edit than the title provided by a <see cref="ITitleFacet" />
        /// </summary>
        /// <para>
        ///     The idea here is that the viewer can display a parseable title
        ///     for an existing object when, for example, the user initially
        ///     clicks in the field.  So, a date might be rendered via a
        ///     <see cref="ITitleFacet" /> as <b>May 2, 2007</b>, but its parseable
        ///     form might be <b>20070502</b>.
        /// </para>
        string ParseableTitle(INakedObjectAdapter nakedObjectAdapter);

        string InvariantString(INakedObjectAdapter nakedObjectAdapter);
    }
}