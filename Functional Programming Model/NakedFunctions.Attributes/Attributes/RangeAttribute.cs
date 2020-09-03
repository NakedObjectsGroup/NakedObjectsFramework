using System;

namespace NakedFunctions
{

    [AttributeUsage(AttributeTargets.Parameter)]
    public class RangeAttribute : System.ComponentModel.DataAnnotations.RangeAttribute
    {
        public RangeAttribute(double minimum, double maximum) : base(minimum, maximum)
        {
        }
    }
}
