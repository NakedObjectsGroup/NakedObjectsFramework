using NakedFunctions;
using System;



namespace AdventureWorksModel
{
    public static class BusinessEntityAddressFunctions
    {

        public static BusinessEntityAddress Updating(BusinessEntityAddress x, [Injected] DateTime now) => x with { ModifiedDate = now };

    }
}
