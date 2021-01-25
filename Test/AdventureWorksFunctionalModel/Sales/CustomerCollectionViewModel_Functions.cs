using AW.Types;
using NakedFunctions;
using System.Linq;

namespace AW.Functions
{
    public static class CustomerCollectionViewModel_Functions
    {

        public static string[] DeriveKeys(CustomerCollectionViewModel vm)
        {
            return vm.Customers.Select(c => c.CustomerID.ToString()).ToArray();
        }

        public static CustomerCollectionViewModel PopulateUsingKeys(CustomerCollectionViewModel vm, string[] keys, IContext context)
        {        
            int[] ids = keys == null ? new int[] { } : keys.Select(int.Parse).ToArray();
            return new CustomerCollectionViewModel((from c in context.Instances<Customer>()
                                              from id in ids
                                              where c.CustomerID == id
                                              select c).ToList());
        }
    }
}
