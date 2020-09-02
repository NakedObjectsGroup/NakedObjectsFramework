using System;

namespace NakedFunctions
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class MultiLineAttribute : Attribute
    {
        private int v;

        public MultiLineAttribute(int v)
        {
            this.v = v;
        }
    }
}
