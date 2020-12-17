using System;

namespace NakedFunctions
{
    /// <summary>
    ///   Applied to a function, indicating that the primary role of the function is to create a new persistent object of the specified type
    ///   with the minimum data set, but with the expectation that the user will likely want to immediately then specify further properties (via Edit methods).
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CreateObjectAttribute : Attribute
    {
    }
}
