using NakedFunctions;
using AW.Types;
using System;

namespace AW.Functions {
    public static class BusinessEntityContactFunctions
    {
        public static BusinessEntityContact Updating(BusinessEntityContact x, IContext context) => x with {ModifiedDate =  context.Now()};
    }
}
