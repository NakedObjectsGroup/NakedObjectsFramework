using System;

namespace NakedFunctions
{
        /// <summary>
        /// Indicates that an action parameter is a password and should therefore be obscured.
        /// </summary>
        [AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
        public class PasswordAttribute : Attribute { }
}
