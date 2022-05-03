// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Error;
using NakedFramework.Metamodel.Serialization;

namespace NakedFramework.Metamodel.Spec;

/// <summary>
///     For base subclasses or, more likely, to help write tests
/// </summary>
[Serializable]
public abstract class Specification : ISpecificationBuilder {

    protected Specification(IIdentifier identifier) {
        Identifier = identifier;
    }

    private readonly FacetDictionarySerializationWrapper facetDictionary = new();

    private void AddFacet(Type facetType, IFacet facet) {
        var existingFacet = GetFacet(facetType);

        if (facet.IsNoOp && existingFacet is { IsNoOp: false }) {
            return;
        }

        if (existingFacet is null || existingFacet.IsNoOp || facet.CanAlwaysReplace) {
            if (existingFacet is { CanNeverBeReplaced : true }) {
                throw new ReflectionException($"Attempting to replace non-replaceable {existingFacet} with {facet}");
            }

            facetDictionary.AddFacet(facet);
        }
    }

    #region ISpecificationBuilder Members

    public virtual Type[] FacetTypes => facetDictionary.Keys.ToArray();

    public virtual IIdentifier Identifier { get; }

    public bool ContainsFacet(Type facetType) => GetFacet(facetType) is not null;

    public bool ContainsFacet<T>() where T : IFacet => ContainsFacet(typeof(T));

    public virtual IFacet GetFacet(Type facetType) => facetDictionary.GetFacet(facetType);

    public T GetFacet<T>() where T : IFacet => (T)GetFacet(typeof(T));

    public virtual IEnumerable<IFacet> GetFacets() => facetDictionary.Values;

    public virtual void AddFacet(IFacet facet) => AddFacet(facet.FacetType, facet);

    public void RemoveFacet(IFacet facet) {
        if (ContainsFacet(facet.FacetType)) {
            facetDictionary.RemoveFacet(facet);
        }
    }

    #endregion
}