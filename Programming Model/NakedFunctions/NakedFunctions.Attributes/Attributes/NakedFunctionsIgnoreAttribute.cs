using System;

namespace NakedFunctions
{
    /// <summary>
    /// Tells the framework not to reflect over a  property or function and hence not incorporate it into the metamodel.
    /// Typically when a property/function must be public, but uses a type that will not be in the metamodel.
    /// </summary>
        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
        public class NakedFunctionsIgnoreAttribute : Attribute { }
}
