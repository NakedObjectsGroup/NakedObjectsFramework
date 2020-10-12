using System;

namespace NakedFunctions
{
    /// <summary>
    /// Specify the minimum and maximum allowed values for a numeric field, or the minimum and maximum
    /// number of characters for a string field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class RangeAttribute : System.ComponentModel.DataAnnotations.RangeAttribute
    {
        public RangeAttribute(int minimum, int maximum = int.MaxValue) : base(minimum, maximum)
        {
        }
        public double MinInt { get; private set; }
        public double MaxInt { get; private set; }
    }
}
