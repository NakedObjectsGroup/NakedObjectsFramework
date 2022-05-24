using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
using Apex.Serialization.Extensions;
using NakedFramework.Architecture.Facet;
using NakedFramework.Core.Configuration;

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

    public static void Write(FacetDictionarySerializationWrapper obj, IBinaryWriter writer)
    {
        obj.OnSerializing(new StreamingContext(StreamingContextStates.All));
        writer.WriteObject(obj.serializeKeyList);
        writer.WriteObject(obj.serializeValueList);
    }

    public static void Read(FacetDictionarySerializationWrapper obj, IBinaryReader reader)
    {
        
        var keylist = reader.ReadObject<List<TypeSerializationWrapper>>();
        var valuelist = reader.ReadObject<List<IFacet>>();

        obj.serializeKeyList = keylist;
        obj.serializeValueList = valuelist;

        obj.OnDeserialized(new StreamingContext(StreamingContextStates.All));
    }
}