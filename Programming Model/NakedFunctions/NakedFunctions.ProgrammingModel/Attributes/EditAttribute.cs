using System;

namespace NakedFunctions
{
    /// <summary>
    ///   Applied to a function, indicating that the function is intended to allow the user to edit the value of one or more properties 
    ///   on the type the function is contributed to.  The function's paramaters (after the first, 'contributee, parameter) must match
    ///   properties on the contributee type, both in type and name (except for casing).
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class EditAttribute : Attribute
    {
    }

}
