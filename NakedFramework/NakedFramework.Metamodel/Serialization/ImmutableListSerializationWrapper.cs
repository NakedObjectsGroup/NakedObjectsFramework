using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;

namespace NakedFramework.Metamodel.Serialization;

[Serializable]
public sealed class ImmutableListSerializationWrapper<T> {
    private readonly bool jit;

    private readonly List<T> list;

    [NonSerialized]
    private ImmutableList<T> immutableList;

    public ImmutableListSerializationWrapper(ImmutableList<T> immutableList, bool jit) {
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