using NakedFunctions;
using AW.Types;
using System;

namespace AW.Functions
{
    public static class PersonPhone_Functions
    {
        public static PersonPhone Updating(PersonPhone x, IContext context) => x with { ModifiedDate = context.Now() };
    }
}
