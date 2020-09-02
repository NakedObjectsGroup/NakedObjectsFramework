using System;

namespace NakedFunctions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Parameter)]
    public class DescribedAsAttribute : Attribute
    {
        private string v;

        public DescribedAsAttribute(string v)
        {
            this.v = v;
        }
    }
}
