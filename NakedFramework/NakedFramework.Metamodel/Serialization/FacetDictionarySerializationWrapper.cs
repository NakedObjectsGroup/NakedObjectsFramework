using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
using NakedFramework.Architecture.Facet;

namespace NakedFramework.Metamodel.Serialization;

[Serializable]
public sealed class FacetDictionarySerializationWrapper {
    [NonSerialized]
    private IImmutableDictionary<Type, IFacet> facetsByClass = ImmutableDictionary<Type, IFacet>.Empty;

    private List<TypeSerializationWrapper> serializeKeyList;
    private List<IFacet> serializeValueList;

    public IEnumerable<Type> Keys => facetsByClass.Keys;

    public IEnumerable<IFacet> Values => facetsByClass.Values;

    public bool ContainsFacet(Type facetType) => facetsByClass.ContainsKey(facetType);

    public IFacet GetFacet(Type facetType) => facetsByClass.ContainsKey(facetType) ? facetsByClass[facetType] : null;

    public void AddFacet(IFacet facet) => facetsByClass = facetsByClass.SetItem(facet.FacetType, facet);

    public void RemoveFacet(IFacet facet) => facetsByClass = facetsByClass.Remove(facet.FacetType);

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context) {
        facetsByClass = serializeKeyList.Zip(serializeValueList).ToDictionary(t => t.First.Type, t => t.Second).ToImmutableDictionary();
    }

    [OnSerializing]
    private void OnSerializing(StreamingContext context) {
        serializeKeyList = facetsByClass.Keys.Select(k => TypeSerializationWrapper.Wrap(k)).ToList();
        serializeValueList = facetsByClass.Values.ToList();
    }
}