using NakedFunctions;
using System;

namespace AdventureWorksModel {
    public static class BusinessEntityContactFunctions
    {
        public static BusinessEntityContact Updating(BusinessEntityContact x, [Injected] DateTime now) => x with {ModifiedDate =  now};
    }
}
