using AW.Types;
using NakedFunctions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AW.Functions
{
    public static class CustomerCollectionViewModel_Functions
    {

        public static string[] DeriveKeys(CustomerCollectionViewModel vm)
        {
            return vm.Customers.Select(c => c.CustomerID.ToString()).ToArray();
        }

        public static CustomerCollectionViewModel PopulateUsingKeys(
            CustomerCollectionViewModel vm,
            string[] keys,
            IQueryable<Customer> customers)
        {
            throw new NotImplementedException();
            //int[] ids = keys == null ? new int[] { } : keys.Select(int.Parse).ToArray();
            //return vm.With(x => x.Customers, (from c in customers
            //             from id in ids
            //             where c.CustomerID == id
            //             select c).ToList());
        }
    }
}
