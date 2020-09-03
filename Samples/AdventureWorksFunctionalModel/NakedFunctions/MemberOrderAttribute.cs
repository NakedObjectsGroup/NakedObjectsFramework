using System;

namespace NakedFunctions
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class MemberOrderAttribute : Attribute
    {
        private int v;
        private string subMenu;

        public MemberOrderAttribute(int v)
        {
            this.v = v;
        }

        public MemberOrderAttribute(int v, string subMenu)
        {
            this.v = v;
            this.subMenu = subMenu;
        }
    }
}
