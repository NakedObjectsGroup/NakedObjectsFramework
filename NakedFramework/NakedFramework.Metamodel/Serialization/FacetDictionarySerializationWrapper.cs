using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
using NakedFramework.Architecture.Facet;
using NakedFramework.Core.Configuration;

namespace NakedFramework.Metamodel.Serialization;

[Serializable]
public class FacetDictionarySerializationWrapper {
    [NonSerialized]
    private IImmutableDictionary<Type, IFacet> facetsByClass = ImmutableDictionary<Type, IFacet>.Empty;

    private List<(TypeSerializationWrapper key, IFacet value)> serializeList;

    public IEnumerable<Type> Keys => facetsByClass.Keys;

    public IEnumerable<IFacet> Values => facetsByClass.Values;

    public bool ContainsFacet(Type facetType) => facetsByClass.ContainsKey(facetType);

    public IFacet GetFacet(Type facetType) => facetsByClass.ContainsKey(facetType) ? facetsByClass[facetType] : null;

    public void AddFacet(IFacet facet) => facetsByClass = facetsByClass.SetItem(facet.FacetType, facet);

    public void RemoveFacet(IFacet facet) => facetsByClass = facetsByClass.Remove(facet.FacetType);

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context) => facetsByClass = serializeList.ToDictionary(t => t.key.Type, t => t.value).ToImmutableDictionary();

    [OnSerializing]
    private void OnSerializing(StreamingContext context) => serializeList = facetsByClass.Select(kvp => (new TypeSerializationWrapper(kvp.Key, ReflectorDefaults.JitSerialization), kvp.Value)).ToList();
}