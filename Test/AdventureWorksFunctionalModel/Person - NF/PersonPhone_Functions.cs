using NakedFunctions;
using System;

namespace AdventureWorksModel
{
    public static class PersonPhone_Functions
    {
        public static PersonPhone Updating(PersonPhone x, IContext context) => x with { ModifiedDate = context.Now() };
    }
}
