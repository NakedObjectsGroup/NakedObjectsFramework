using System;

namespace NakedFunctions
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class RegExAttribute : Attribute
    {
        private string v;

        public RegExAttribute(string v)
        {
            this.v = v;
        }
    }
}
