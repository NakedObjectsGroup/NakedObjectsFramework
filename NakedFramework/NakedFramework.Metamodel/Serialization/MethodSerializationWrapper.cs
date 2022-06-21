using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Utils;

namespace NakedFramework.Metamodel.Serialization;

[Serializable]
public sealed class MethodSerializationWrapper {
    private static Dictionary<MethodInfo, MethodSerializationWrapper> cache = new();
    private readonly bool jit;

    [NonSerialized]
    private readonly Type[] methodArgs;

    private readonly TypeSerializationWrapper[] methodArgsWrapper;
    private readonly string methodName;
    private readonly TypeSerializationWrapper typeWrapper;

    [NonSerialized]
    private MethodInfo method;

    [NonSerialized]
    private Func<object, object[], object> methodDelegate;

    internal MethodSerializationWrapper(MethodInfo methodInfo, ILogger logger, bool jit) {
        this.jit = jit;
        MethodInfo = methodInfo;
        typeWrapper = SerializationFactory.Wrap(methodInfo.DeclaringType);
        methodName = methodInfo.Name;
        methodDelegate = FacetUtils.LogNull(DelegateUtils.CreateDelegate(methodInfo), logger);
    }

    internal MethodSerializationWrapper(MethodInfo methodInfo, Type[] methodArgs, ILogger logger, bool jit) : this(methodInfo, logger, jit) {
        this.methodArgs = methodArgs;
        methodArgsWrapper = methodArgs.Select(a => SerializationFactory.Wrap(a)).ToArray();
    }

    public MethodInfo MethodInfo {
        get => method ??= FindMethod();
        set => method = value;
    }

    public Func<object, object[], object> MethodDelegate {
        get => methodDelegate ??= CreateDelegate();
        set => methodDelegate = value;
    }

    private Func<object, object[], object> CreateDelegate() => DelegateUtils.CreateDelegate(GetMethod()).Item1;

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context) {
        if (!jit) {
            MethodInfo = FindMethod();
            MethodDelegate = CreateDelegate();
        }
    }

    private MethodInfo FindMethod() {
        try {
            var declaringType = typeWrapper.Type;
            var args = methodArgsWrapper?.Select(w => w.Type).ToArray();
            return args is null ? declaringType?.GetMethod(methodName) : declaringType?.GetMethod(methodName, args) ?? throw new NullReferenceException();
        }
        catch (NullReferenceException) {
            throw new ReflectionException($"Failed to find {methodName}");
        }
    }

    public MethodInfo GetMethod() => MethodInfo;

    private static object[] Args(INakedObjectAdapter[] args) => args.Select(a => a.GetDomainObject()).ToArray();

    public Func<object, object[], object> GetMethodDelegate() => MethodDelegate;

    public T Invoke<T>(object target, object[] args) => MethodDelegate.Invoke<T>(MethodInfo, target, args);

    public T Invoke<T>(object target) => MethodDelegate.Invoke<T>(MethodInfo, target, null);

    public T Invoke<T>(INakedObjectAdapter target, INakedObjectAdapter[] args) => MethodDelegate.Invoke<T>(MethodInfo, target.GetDomainObject(), Args(args));

    public T Invoke<T>(object[] args) => MethodDelegate.InvokeStatic<T>(MethodInfo, args);

    public void Invoke(object target, object[] args) => MethodDelegate.Invoke(MethodInfo, target, args);

    public void Invoke(object target) => MethodDelegate.Invoke(MethodInfo, target, null);

    public void Invoke(object[] args) => MethodDelegate.InvokeStatic(MethodInfo, args);
}