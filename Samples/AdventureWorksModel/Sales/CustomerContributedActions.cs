// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NakedObjects;
using NakedObjects.Services;

namespace AdventureWorksModel {
    public class CustomerCollectionViewModel : IViewModel {
        public IDomainObjectContainer Container { protected get; set; }
        public IList<Customer> Customers { get; set; }

        public string[] DeriveKeys() {
            return Customers.Select(c => c.Id.ToString()).ToArray();
        }

        public void PopulateUsingKeys(string[] instanceId) {
            int[] ids = instanceId == null ? new int[] {} : instanceId.Select(int.Parse).ToArray();

            Customers = (from c in Container.Instances<Customer>()
                         from id in ids
                         where c.Id == id
                         select c).ToList();
        }
    }

    [DisplayName("Customers")]
    public class CustomerContributedActions : AbstractFactoryAndRepository {
        [QueryOnly]
        public CustomerCollectionViewModel ShowCustomersWithAddressInRegion(CountryRegion region, [ContributedAction] IQueryable<Customer> customers) {
            List<Customer> cc = customers.Where(c => c.Addresses.Any(a => a.Address.StateProvince.CountryRegion == region)).ToList();
            var ccvm = Container.NewViewModel<CustomerCollectionViewModel>();
            ccvm.Customers = cc.ToList();
            return ccvm;
        }
    }
}