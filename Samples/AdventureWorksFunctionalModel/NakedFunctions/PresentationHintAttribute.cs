using System;

namespace NakedFunctions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PresentationHintAttribute : Attribute
    {
        private string v;

        public PresentationHintAttribute(string v)
        {
            this.v = v;
        }
    }
}
