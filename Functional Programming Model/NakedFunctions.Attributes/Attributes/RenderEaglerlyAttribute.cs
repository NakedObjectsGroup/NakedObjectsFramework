using System;

namespace NakedFunctions
{
    /// <summary>
    ///     Indicates that a property or all the properties of a class are to be eagerly rendered.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
        public class RenderEagerlyAttribute : Attribute { }
}
