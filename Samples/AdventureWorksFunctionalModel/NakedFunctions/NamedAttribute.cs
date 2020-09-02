using System;

namespace NakedFunctions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Parameter)]
    public class NamedAttribute : Attribute
    {
        private string v;

        public NamedAttribute(string v)
        {
            this.v = v;
        }
    }
}
