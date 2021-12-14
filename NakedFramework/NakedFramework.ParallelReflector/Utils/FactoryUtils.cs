using System;

namespace NakedFramework.ParallelReflector.Utils;

public static class FactoryUtils {
    public static bool IsVoid(Type type) => type == typeof(void);

    public static bool IsSealed(Type type) => type.IsSealed;

    public static bool IsInterface(Type type) => type.IsInterface;

    public static bool IsAbstract(Type type) => type.IsAbstract;

    public static bool IsStatic(Type type) => IsAbstract(type) && IsSealed(type);


}