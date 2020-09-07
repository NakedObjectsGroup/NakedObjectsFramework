using System;

namespace NakedFunctions
{
    /// <summary>
    ///     A hint added to the associated display element. For example to be rendered as a class on the html, and picked up by the CSS.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class PresentationHintAttribute : Attribute
    {
        public PresentationHintAttribute(string s)
        {
            Value = s;
        }

        public string Value { get; private set; }
    }
}
