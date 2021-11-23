






using System;
using System.Linq;
using AW.Types;
using NakedFunctions;

namespace AW.Functions {
    [Named("Customers")]
    public static class CustomerQuery_Functions {
        public static CustomerCollectionViewModel ShowCustomersWithAddressInRegion(this IQueryable<Customer> customers, CountryRegion region) => throw new NotImplementedException();
    }
}