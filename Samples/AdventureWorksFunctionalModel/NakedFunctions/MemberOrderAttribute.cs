using System;

namespace NakedFunctions
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class MemberOrderAttribute : Attribute
    {
        private int v;

        public MemberOrderAttribute(int v)
        {
            this.v = v;
        }
    }
}
