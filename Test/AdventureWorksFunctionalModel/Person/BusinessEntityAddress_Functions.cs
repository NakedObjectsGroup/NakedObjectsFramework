using NakedFunctions;

namespace AdventureWorksModel
{
    public static class BusinessEntityAddressFunctions
    {

        public static BusinessEntityAddress Updating(BusinessEntityAddress x, IContext context) => x with { ModifiedDate = context.Now() };

    }
}
