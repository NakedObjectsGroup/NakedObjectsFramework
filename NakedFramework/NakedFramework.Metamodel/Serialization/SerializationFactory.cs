using System;
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Core.Configuration;

namespace NakedFramework.Metamodel.Serialization;

public static class SerializationFactory {
    private static readonly ConcurrentDictionary<Type, TypeSerializationWrapper> TypeCache = new();
    private static readonly ConcurrentDictionary<MethodInfo, MethodSerializationWrapper> MethodCache = new();
    private static readonly ConcurrentDictionary<PropertyInfo, PropertySerializationWrapper> PropertyCache = new();

    public static TypeSerializationWrapper Wrap(Type type) => TypeCache.GetOrAdd(type, t => new TypeSerializationWrapper(type, ReflectorDefaults.JitSerialization));

    public static MethodSerializationWrapper Wrap(MethodInfo method, ILogger logger) => MethodCache.GetOrAdd(method, m => new MethodSerializationWrapper(m, logger, ReflectorDefaults.JitSerialization));

    public static MethodSerializationWrapper Wrap(MethodInfo method, Type[] methodArgs, ILogger logger) => MethodCache.GetOrAdd(method, m => new MethodSerializationWrapper(m, methodArgs, logger, ReflectorDefaults.JitSerialization));

    public static PropertySerializationWrapper Wrap(PropertyInfo property, ILogger logger) => PropertyCache.GetOrAdd(property, p => new PropertySerializationWrapper(p, logger, ReflectorDefaults.JitSerialization));
}