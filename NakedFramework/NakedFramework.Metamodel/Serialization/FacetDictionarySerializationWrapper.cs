using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
using NakedFramework.Architecture.Facet;

namespace NakedFramework.Metamodel.Serialization;

[Serializable]
public class FacetDictionarySerializationWrapper {
    [NonSerialized]
    private IImmutableDictionary<Type, IFacet> facetsByClass = ImmutableDictionary<Type, IFacet>.Empty;

    private Dictionary<TypeSerializationWrapper, IFacet> serializeDictionary;

    public IEnumerable<Type> Keys => facetsByClass.Keys;

    public IEnumerable<IFacet> Values => facetsByClass.Values;

    public bool ContainsFacet(Type facetType) => facetsByClass.ContainsKey(facetType);

    public IFacet GetFacet(Type facetType) => facetsByClass.ContainsKey(facetType) ? facetsByClass[facetType] : null;

    public void AddFacet(IFacet facet) => facetsByClass = facetsByClass.SetItem(facet.FacetType, facet);

    public void RemoveFacet(IFacet facet) => facetsByClass = facetsByClass.Remove(facet.FacetType);

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context) => facetsByClass = serializeDictionary.ToDictionary(kvp => kvp.Key.Type, kvp => kvp.Value).ToImmutableDictionary();

    [OnSerializing]
    private void OnSerializing(StreamingContext context) => serializeDictionary = facetsByClass.ToDictionary(kvp => new TypeSerializationWrapper(kvp.Key), kvp => kvp.Value);
}