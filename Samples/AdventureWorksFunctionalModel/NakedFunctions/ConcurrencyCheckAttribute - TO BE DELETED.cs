using System;

namespace NakedFunctions
{
        [AttributeUsage(AttributeTargets.Property)]
        public class ConcurrencyCheckAttribute : Attribute { }
}
