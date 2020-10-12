using System;

namespace NakedFunctions
{
    /// <summary>
    /// Indicates that a method is idempotent (such that e.g. it could be accessed via an RO resource with a n Http PUT method)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class IdempotentAttribute : Attribute { }
}