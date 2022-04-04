using System;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Utils;

namespace NakedFramework.Metamodel.Serialization;

[Serializable]
public class MethodSerializationWrapper {
    private readonly bool jit;
    private readonly string methodName;
    private readonly TypeSerializationWrapper typeWrapper;

    [NonSerialized]
    private MethodInfo method;

    [NonSerialized]
    private Func<object, object[], object> methodDelegate;

    public MethodSerializationWrapper(MethodInfo methodInfo, ILogger logger, bool jit = false) {
        this.jit = jit;
        MethodInfo = methodInfo;
        typeWrapper = new TypeSerializationWrapper(methodInfo.DeclaringType, jit);
        methodName = methodInfo.Name;
        methodDelegate = FacetUtils.LogNull(DelegateUtils.CreateDelegate(methodInfo), logger);
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
            return declaringType?.GetMethod(methodName) ?? throw new NullReferenceException();
        }
        catch (NullReferenceException) {
            throw new ReflectionException($"Failed to find {methodName}");
        }
    }

    public MethodInfo GetMethod() => MethodInfo;

    public Func<object, object[], object> GetMethodDelegate() => MethodDelegate;
}