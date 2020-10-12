using System;

namespace NakedFunctions
{
    /// <summary>
    ///     Tells framework that this class has a 'bounded' (limited) number of instances, such that they could all be represented e.g. in a drop-down list at the UI
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
        public class BoundedAttribute : Attribute { }
}
