using System;

namespace NakedFunctions
{
    /// <summary>
    ///     Indicates that a collection property, or all the collection properties of a class, are to rendered 'opened' by default
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method)]
        public class RenderEagerlyAttribute : Attribute { }
}
