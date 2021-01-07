using NakedFunctions;
using AW.Types;

namespace AW.Functions
{
    public static class BusinessEntityAddressFunctions
    {

        public static BusinessEntityAddress Updating(BusinessEntityAddress x, IContext context) => x with { ModifiedDate = context.Now() };

    }
}
