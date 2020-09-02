using System;

namespace NakedFunctions
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class TableViewAttribute : Attribute
    {
        private bool v;
        private string[] v2;

        public TableViewAttribute(bool v)
        {
            this.v = v;
        }

        public TableViewAttribute(bool v,params string[] v2)
        {
            this.v = v;
            this.v2 = v2;
        }
    }
}
