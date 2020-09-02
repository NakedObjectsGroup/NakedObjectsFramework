using System;

namespace NakedFunctions
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PageSizeAttribute : Attribute
    {
        private int v;

        public PageSizeAttribute(int v)
        {
            this.v = v;
        }
    }
}
