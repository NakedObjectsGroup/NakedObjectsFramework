using System;

namespace NakedFunctions
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class DefaultValueAttribute : Attribute
    {
        private bool v;

        public DefaultValueAttribute(bool v)
        {
            this.v = v;
        }
    }
}
