﻿using System;
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
    private Func<object, object[], object> methodDelegate;

    [NonSerialized]
    private PropertyInfo property;

    public PropertySerializationWrapper(PropertyInfo propertyInfo, ILogger logger, bool jit = false) {
        this.jit = jit;
        PropertyInfo = propertyInfo;
        typeWrapper = new TypeSerializationWrapper(propertyInfo.DeclaringType, jit);
        propertyName = propertyInfo.Name;
        methodDelegate = FacetUtils.LogNull(DelegateUtils.CreateDelegate(propertyInfo.GetGetMethod()), logger);
    }

    public PropertyInfo PropertyInfo {
        get => property ??= FindProperty();
        set => property = value;
    }

    public Func<object, object[], object> MethodDelegate {
        get => methodDelegate ??= CreateDelegate();
        set => methodDelegate = value;
    }

    private Func<object, object[], object> CreateDelegate() => DelegateUtils.CreateDelegate(GetMethod()).Item1;

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context) {
        if (!jit) {
            PropertyInfo = FindProperty();
            MethodDelegate = CreateDelegate();
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

    public Func<object, object[], object> GetMethodDelegate() => MethodDelegate;
}