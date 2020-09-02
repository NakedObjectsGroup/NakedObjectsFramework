using System;

namespace NakedFunctions
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class MaskAttribute : Attribute
    {
        private string v;

        public MaskAttribute(string v)
        {
            this.v = v;
        }
    }
}
