using NakedFunctions;

namespace AdventureWorksModel
{
    public static class BusinessEntityAddressFunctions
    {

        public static BusinessEntityAddress Updating(BusinessEntityAddress x, IContainer container) => x with { ModifiedDate = container.Now() };

    }
}
