using System;

namespace NakedFunctions
{
    /// <summary>
    /// Applied to a string parameter, specifies the minimum & maximum string length that the user may enter.
    /// When applied to the string parameter of an AutoComplete method, the Minimum specifies the number of
    /// characters the user must type before the method will be invoked.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class LengthAttribute : Attribute
    {
        public LengthAttribute(int minimum, int maximum = int.MaxValue)
        {
            MinInt = minimum;
            MaxInt = maximum;
        }
        public double MinInt { get; private set; }
        public double MaxInt { get; private set; }
    }
}
