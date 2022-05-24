using System;
using System.Linq;
using System.Runtime.Serialization;
using NakedFramework.Core.Error;

namespace NakedFramework.Metamodel.Serialization;

[Serializable]
public sealed class TypeSerializationWrapper  {
    private readonly string assemblyName;
    private readonly bool jit;
    private readonly string typeName;

    [NonSerialized]
    private Type type;

    public TypeSerializationWrapper(Type type, bool jit) {
        this.jit = jit;
        this.type = type;
        assemblyName = type.Assembly.FullName;
        typeName = type.FullName;
    }

    public Type Type {
        get => type ??= FindType();
        set => type = value;
    }

   

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context) {
        if (!jit) {
            Type = FindType();
        }
    }

    private Type FindType() => FindType(assemblyName, typeName);

    private static Type FindType(string an, string tn) {
        try {
            return AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.FullName == an)?.GetType(tn) ?? throw new NullReferenceException();
        }
        catch (NullReferenceException) {
            throw new ReflectionException($"Failed to find {an}:{tn}");
        }
    }

  

}