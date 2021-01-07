using NakedFunctions;
using AW.Types;

namespace AW.Functions
{
    public static class PhoneNumberTypeFunctions
    {
        public static PhoneNumberType Updating(PhoneNumberType pnt, IContext context) => pnt with { ModifiedDate = context.Now() };
    }
}
