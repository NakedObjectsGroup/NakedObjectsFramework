using System;

namespace NakedFunctions
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class RangeAttribute : Attribute
    {
        private int v1;
        private int v2;

        public RangeAttribute(int v1, int v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }
    }
}
