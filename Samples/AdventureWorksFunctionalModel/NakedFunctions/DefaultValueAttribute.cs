using System;

namespace NakedFunctions
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class DefaultValueAttribute : Attribute
    {
        //TODO: Check this against the signature of the System.ComponentModel equivalent attribute
        private object v;

        public DefaultValueAttribute(object v)
        {
            this.v = v;
        }
    }
}
