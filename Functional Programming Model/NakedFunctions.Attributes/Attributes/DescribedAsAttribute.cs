using System;

namespace NakedFunctions
{
    /// <summary>
    /// Provides a user-oriented description for an item that might e.g. be displayed as a tooltip.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class DescribedAsAttribute : Attribute
    {
        public DescribedAsAttribute(string s)
        {
            Value = s;
        }

        public string Value { get; private set; }
    }
}
