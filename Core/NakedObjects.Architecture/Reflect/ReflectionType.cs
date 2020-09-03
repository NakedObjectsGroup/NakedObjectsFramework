using System;

namespace NakedObjects.Architecture.Reflect {
    [Flags]
    public enum ReflectionType {
        None = 0,
        ObjectOriented = 1,
        Functional = 2,
        Both = ObjectOriented | Functional
    }
}