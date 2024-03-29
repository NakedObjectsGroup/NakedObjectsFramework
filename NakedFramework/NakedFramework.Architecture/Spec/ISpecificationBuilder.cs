// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFramework.Architecture.Facet;

namespace NakedFramework.Architecture.Spec;

public interface ISpecificationBuilder : ISpecification {
    /// <summary>
    ///     Adds the facet, extracting its <see cref="IFacet.FacetType" /> as the key.
    /// </summary>
    /// <para>
    ///     If there are any facet of the same type, they will be overwritten <i>provided</i>
    ///     that either the <see cref="IFacet" /> specifies to <see cref="IFacet.CanAlwaysReplace" />
    ///     or if the existing <see cref="IFacet" /> is an <see cref="IFacet.IsNoOp" />
    /// </para>
    void AddFacet(IFacet facet);

    void RemoveFacet(IFacet facet);
}