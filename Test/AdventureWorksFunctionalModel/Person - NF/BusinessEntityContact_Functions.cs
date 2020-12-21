using NakedFunctions;
using System;

namespace AdventureWorksModel {
    public static class BusinessEntityContactFunctions
    {
        public static BusinessEntityContact Updating(BusinessEntityContact x, IContainer container) => x with {ModifiedDate =  container.Now()};
    }
}
