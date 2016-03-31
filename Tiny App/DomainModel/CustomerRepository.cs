using NakedObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel
{
    public class CustomerRepository
    {
        #region Injected Services
        //An implementation of this interface is injected automatically by the framework
        public IDomainObjectContainer Container { set; protected get; }
        #endregion
        public Customer CreateNewCustomer()
        {
            return Container.NewTransientInstance<Customer>();
        }

        public IQueryable<Customer> AllCustomers()
        {
            return Container.Instances<Customer>();
        }
    }

}
