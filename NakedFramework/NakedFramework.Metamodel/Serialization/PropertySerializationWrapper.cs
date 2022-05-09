using System;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Utils;

namespace NakedFramework.Metamodel.Serialization;

[Serializable]
public class PropertySerializationWrapper {
    private readonly bool jit;
    private readonly string propertyName;
    private readonly TypeSerializationWrapper typeWrapper;

    [NonSerialized]
    private Func<object, object[], object> getMethodDelegate;

    [NonSerialized]
    private PropertyInfo property;

    [NonSerialized]
    private Func<object, object[], object> setMethodDelegate;

    public PropertySerializationWrapper(PropertyInfo propertyInfo, ILogger logger, bool jit = false) {
        this.jit = jit;
        PropertyInfo = propertyInfo;
        typeWrapper = new TypeSerializationWrapper(propertyInfo.DeclaringType, jit);
        propertyName = propertyInfo.Name;
        getMethodDelegate = FacetUtils.LogNull(DelegateUtils.CreateDelegate(propertyInfo.GetGetMethod()), logger);
    }

    public PropertyInfo PropertyInfo {
        get => property ??= FindProperty();
        set => property = value;
    }

    public T GetValue<T>(object target) => (T)PropertyInfo.GetValue(target);


    public Func<object, object[], object> GetMethodDelegate {
        get => getMethodDelegate ??= CreateGetDelegate();
        set => getMethodDelegate = value;
    }

    public Func<object, object[], object> SetMethodDelegate {
        get => setMethodDelegate ??= CreateSetDelegate();
        set => setMethodDelegate = value;
    }

    private Func<object, object[], object> CreateGetDelegate() {
        var getMethod = GetMethod();
        return getMethod is not null ? DelegateUtils.CreateDelegate(getMethod).Item1 : null;
    }

    private Func<object, object[], object> CreateSetDelegate() {
        var setMethod = SetMethod();
        return setMethod is not null ?  DelegateUtils.CreateDelegate(setMethod).Item1 : null;
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context) {
        if (!jit) {
            PropertyInfo = FindProperty();
            GetMethodDelegate = CreateGetDelegate();
            SetMethodDelegate = CreateSetDelegate();
        }
    }

    private PropertyInfo FindProperty() {
        try {
            var declaringType = typeWrapper.Type;
            return declaringType?.GetProperty(propertyName) ?? throw new NullReferenceException();
        }
        catch (NullReferenceException) {
            throw new ReflectionException($"Failed to find {propertyName}");
        }
    }

    public MethodInfo GetMethod() => PropertyInfo.GetGetMethod();

    public MethodInfo SetMethod() => PropertyInfo.GetSetMethod();

    public Func<object, object[], object> GetGetMethodDelegate() => GetMethodDelegate;

    public Func<object, object[], object> GetSetMethodDelegate() => SetMethodDelegate;
}