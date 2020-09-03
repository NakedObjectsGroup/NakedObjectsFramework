using System;

namespace NakedFunctions
{
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
