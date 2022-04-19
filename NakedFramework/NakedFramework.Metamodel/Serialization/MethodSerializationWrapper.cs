using System;
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
public class MethodSerializationWrapper {
    private readonly TypeSerializationWrapper[] methodArgsWrapper;
    private readonly bool jit;
    private readonly string methodName;
    private readonly TypeSerializationWrapper typeWrapper;

    [NonSerialized]
    private MethodInfo method;

    [NonSerialized]
    private Func<object, object[], object> methodDelegate;

    [NonSerialized]
    private readonly Type[] methodArgs;

    public MethodSerializationWrapper(MethodInfo methodInfo, ILogger logger, bool jit = false) {
        this.jit = jit;
        MethodInfo = methodInfo;
        typeWrapper = new TypeSerializationWrapper(methodInfo.DeclaringType, jit);
        methodName = methodInfo.Name;
        methodDelegate = FacetUtils.LogNull(DelegateUtils.CreateDelegate(methodInfo), logger);
    }

    public MethodSerializationWrapper(MethodInfo methodInfo, ILogger logger, Type[] methodArgs, bool jit = false) : this(methodInfo, logger, jit) {
        this.methodArgs = methodArgs;
        methodArgsWrapper = methodArgs.Select(a => new TypeSerializationWrapper(a, jit)).ToArray();
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

    public T Invoke<T>(INakedObjectAdapter target, INakedObjectAdapter[] args) => MethodDelegate.Invoke<T>(MethodInfo, target.GetDomainObject(), Args(args));

    public T Invoke<T>(object[] args) => MethodDelegate.InvokeStatic<T>(MethodInfo, args);

    public void Invoke(object target, object[] args) => MethodDelegate.Invoke(MethodInfo, target, args);

    public void Invoke(object target) => MethodDelegate.Invoke(MethodInfo, target, null);

    public void Invoke(object[] args) => MethodDelegate.InvokeStatic(MethodInfo, args);
}