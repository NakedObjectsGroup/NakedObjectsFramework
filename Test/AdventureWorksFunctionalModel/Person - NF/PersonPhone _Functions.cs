using NakedFunctions;
using System;
using System.ComponentModel.DataAnnotations;

namespace AdventureWorksModel
{
    public static class PersonPhone_Functions
    {

        public static PersonPhone Updating(PersonPhone x, [Injected] DateTime now) => x with { ModifiedDate = now };
    }
}
