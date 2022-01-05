using System;
using System.Linq;
using NakedLegacy.Types;

namespace NakedLegacy.Reflector.Helpers;

public static class LegacyHelpers {
    public static Type IsOrImplementsValueHolder(Type type) =>
        type switch {
            null => null,
            _ when type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ValueHolder<>) => type.GetGenericArguments().Single(),
            _ => IsOrImplementsValueHolder(type.BaseType)
        };
}