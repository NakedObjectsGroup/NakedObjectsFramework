using System;

namespace NakedFunctions
{
    /// <summary>
    ///     Used when you want to specify the way something is named on the user interface i.e. when you do
    ///     not want to use the name generated automatically by the system. It can be applied to objects,
    ///     members (properties, collections, and actions) and to parameters within an action method.
    /// </summary>
    /// <seealso cref="PluralAttribute" />
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class NamedAttribute : Attribute
    {
        public NamedAttribute(string s)
        {
            Value = s;
        }

        public string Value { get; private set; }
    }
}
