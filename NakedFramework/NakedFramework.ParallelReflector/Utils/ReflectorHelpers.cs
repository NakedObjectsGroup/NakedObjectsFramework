using System;
using System.Linq;

namespace NakedFramework.ParallelReflector.Utils;

public static class ReflectorHelpers {
    public static Type EnsureGenericTypeIsComplete(Type type) {
        if (type.IsGenericType && !type.IsConstructedGenericType) {
            var genericType = type.GetGenericTypeDefinition();
            var genericParms = genericType.GetGenericArguments().Select(a => typeof(object)).ToArray();

            return type.GetGenericTypeDefinition().MakeGenericType(genericParms);
        }

        return type;
    }
}