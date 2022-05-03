using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
using NakedFramework.Architecture.Spec;

namespace NakedFramework.Metamodel.Serialization;

[Serializable]
public class ImmutableListSerializationWrapper<T> where T : ISpecification {
    private readonly bool jit;

    [NonSerialized]
    private ImmutableList<T> immutableList;

    private readonly List<T> list;

    public ImmutableListSerializationWrapper(ImmutableList<T> immutableList, bool jit = false) {
        this.jit = jit;
        this.immutableList = immutableList;
        list = immutableList.ToList();
    }

    public ImmutableList<T> ImmutableList {
        get => immutableList ??= list.ToImmutableList();
        set => immutableList = value;
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context) {
        if (!jit) {
            ImmutableList = list.ToImmutableList();
        }
    }
}