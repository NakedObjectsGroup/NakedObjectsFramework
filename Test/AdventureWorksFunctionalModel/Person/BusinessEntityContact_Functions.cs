using NakedFunctions;
using System;

namespace AdventureWorksModel {
    public static class BusinessEntityContactFunctions
    {
        public static BusinessEntityContact Updating(BusinessEntityContact x, IContext context) => x with {ModifiedDate =  context.Now()};
    }
}
