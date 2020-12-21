using NakedFunctions;
using System;

namespace AdventureWorksModel
{
    public static class PhoneNumberTypeFunctions
    {
        public static PhoneNumberType Updating(PhoneNumberType pnt, IContext context) => pnt with { ModifiedDate = context.Now() };
    }
}
