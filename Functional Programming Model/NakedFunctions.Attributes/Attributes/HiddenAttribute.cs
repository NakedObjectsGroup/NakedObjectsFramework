using System;

namespace NakedFunctions
{
        /// <summary>
        /// Ensures that a property is never displayed to the user
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class HiddenAttribute : Attribute { }
}
