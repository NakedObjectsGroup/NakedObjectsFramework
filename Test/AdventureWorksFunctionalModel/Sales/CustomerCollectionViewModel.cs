using NakedFunctions;
using System.Collections.Generic;
using AW.Functions;

namespace AW.Types
{
    [ViewModel(typeof(CustomerCollectionViewModel_Functions))]
    public record CustomerCollectionViewModel
    {
        public CustomerCollectionViewModel(IList<Customer> customers)
        {
            Customers = customers;
        }
        public IList<Customer> Customers { get; init; }
    }
}
