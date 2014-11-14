// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;

namespace NakedObjects.Architecture.Spec {
    /// <summary>
    ///  The 'metamodel' is made up of various implementations of ISpecification, which, in turn, are largely
    ///  made up of Facets.
    /// </summary>
    public interface ISpecification {
        /// <summary>
        ///     Get the list of all facet <see cref="Type" />s that are supported by objects with this specification
        /// </summary>
        Type[] FacetTypes { get; }

        /// <summary>
        ///     Identifier of this <see cref="ISpecification" /> (feature)
        /// </summary>
        IIdentifier Identifier { get; }

        /// <summary>
        ///     Whether there is a facet registered of the specified type.
        /// </summary>
        /// <para>
        ///     Convenience; saves having to <see cref="GetFacet" /> and then check if <c>null</c>.
        /// </para>
        bool ContainsFacet(Type facetType);

        /// <summary>
        ///     Whether there is a facet registered of the specified type.
        /// </summary>
        /// <para>
        ///     Convenience; saves having to <see cref="GetFacet" /> and then check if <c>null</c>.
        /// </para>
        bool ContainsFacet<T>() where T : IFacet;

        /// <summary>
        ///     Get the facet of the specified type (as per the type it reports from <see cref="IFacet.FacetType" />)
        /// </summary>
        IFacet GetFacet(Type type);

        /// <summary>
        ///     Get the facet of type T (as per the type it reports from <see cref="IFacet.FacetType" />)
        /// </summary>
        T GetFacet<T>() where T : IFacet;

        /// <summary>
        ///     Returns all <see cref="IFacet" />s  />
        /// </summary>
        IEnumerable<IFacet> GetFacets();
    }
}