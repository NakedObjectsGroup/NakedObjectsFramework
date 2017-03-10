using NakedObjects;
using System.Linq;


namespace Template.Model
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

        public IQueryable<Customer> FindCustomerByName(string name)
        {
            return AllCustomers().Where(c => c.Name.ToUpper().Contains(name.ToUpper()));
        }

    }

}
