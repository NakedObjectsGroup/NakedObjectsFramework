using AW.Types;
using System;

namespace AW.Functions
{
    public static class StaffSummary_Functions
    {
        public static string[] DeriveKeys(this StaffSummary sum) =>
             new[] { sum.Female.ToString(), sum.Male.ToString() };

        public static StaffSummary CreateFromKeys(string[] keys) =>
          new StaffSummary
          {
              Female = Convert.ToInt32(keys[0]),
              Male = Convert.ToInt32(keys[1])
          };
    }
}
