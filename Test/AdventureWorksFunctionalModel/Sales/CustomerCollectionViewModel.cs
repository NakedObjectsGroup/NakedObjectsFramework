using System.Collections.Generic;
using AW.Functions;
using NakedFunctions;

namespace AW.Types {
    [ViewModel(typeof(CustomerCollectionViewModel_Functions))]
    public record CustomerCollectionViewModel {
        public CustomerCollectionViewModel(IList<Customer> customers) => Customers = customers;

        [Hidden]
        public IList<Customer> Customers { get; init; }
    }
}