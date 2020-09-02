using NakedFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorksModel
{
    [ViewModel]
    public class CustomerCollectionViewModel
    {
        public CustomerCollectionViewModel(IList<Customer> customers)
        {
            Customers = customers;
        }
        public IList<Customer> Customers { get; set; }
    }
    public static class CustomerCollectionViewModelFunctions { 

    public static string[] DeriveKeys(CustomerCollectionViewModel vm)
        {
            return vm.Customers.Select(c => c.CustomerID.ToString()).ToArray();
        }

        public static CustomerCollectionViewModel PopulateUsingKeys(
            CustomerCollectionViewModel vm,
            string[] keys, 
            [Injected] IQueryable<Customer> customers )
        {
            int[] ids = keys == null ? new int[] { } : keys.Select(int.Parse).ToArray();
            return vm.With(x => x.Customers, (from c in customers
                         from id in ids
                         where c.CustomerID == id
                         select c).ToList());
        }
    }
}
