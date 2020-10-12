using System;

namespace NakedFunctions
{
    /// <summary>
    /// Indicates that a method is side-effect fre. This should be applied only where the method would otherwise not be
    /// determined as side-effect free based on its signature.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class SideEffectFreeAttribute : Attribute { }
}