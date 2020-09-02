// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;
using NakedObjects.Menu;
using System;
using System.Collections.Generic;
using System.Windows.Navigation;
using AdventureWorksFunctionalModel;
using NakedFunctions;
using static AdventureWorksModel.CommonFactoryAndRepositoryFunctions;
using static NakedFunctions.Result;


namespace AdventureWorksModel {
    [DisplayName("Customers")]
    public static class CustomerRepository {

        //TODO
        //public static void Menu(IMenu menu) {
        //    menu.AddAction(nameof(FindCustomerByAccountNumber));
        //    menu.CreateSubMenu("Stores")
        //        .AddAction(nameof(FindStoreByName))
        //        .AddAction(nameof(CreateNewStoreCustomer))
        //        .AddAction(nameof(RandomStore));
        //    menu.CreateSubMenu("Individuals")
        //        .AddAction(nameof(FindIndividualCustomerByName))
        //        .AddAction(nameof(CreateNewIndividualCustomer))
        //        .AddAction(nameof(RandomIndividual));
        //    menu.AddAction(nameof(CustomerDashboard));
        //    menu.AddAction(nameof(ThrowDomainException));
        //    menu.AddAction(nameof(FindCustomer));
        //    menu.AddRemainingNativeActions();
        //}

        //public static object ThrowDomainException() {
        //    throw new DomainException("Foo");
        //}

        
        public static CustomerDashboard CustomerDashboard(
            string accountNumber,
            [Injected] IQueryable<Customer> customers) {
             var (cust, _) = FindCustomerByAccountNumber(accountNumber, customers);
            return new CustomerDashboard(cust);
        }

        #region FindCustomerByAccountNumber

        [FinderAction]
        [MemberOrder(10)]
        public static (Customer, string) FindCustomerByAccountNumber(
            [DefaultValue("AW")] string accountNumber, 
            [Injected] IQueryable<Customer> customers)
        {
            IQueryable<Customer> query = from obj in customers
                where obj.AccountNumber == accountNumber
                orderby obj.AccountNumber
                select obj;
            return SingleObjectWarnIfNoMatch(query);
        }

        //public static string ValidateFindCustomerByAccountNumber( string accountNumber) {
        //    return accountNumber.StartsWith("AW")? null : "Account number must start with AW";
        //}

        //Method exists to test auto-complete
        public static Customer FindCustomer([Description("Enter Account Number")] Customer customer)
        {
            return customer;
        }

        //[PageSize(10)]
        //public static IQueryable<Customer> AutoComplete0FindCustomer([MinLength(3)] string matching, [Injected] IQueryable<Customer> customers)
        //{
        //    return customers.Where(c => c.AccountNumber.Contains(matching));
        //}
        #endregion

        #region Stores Menu

        [FinderAction]
        [PageSize(2)]
        [TableView(true, "StoreName", "SalesPerson")] //Table view == List View
            public static IQueryable<Customer> FindStoreByName(
            [Description("partial match")]string name,
            [Injected] IQueryable<Customer> customers,
            [Injected] IQueryable<Store> stores
            ) {
                return from c in customers
                       from s in stores
                where s.Name.ToUpper().Contains(name.ToUpper()) &&
                        c.StoreID == s.BusinessEntityID
                select c;
        }

        //TODO:  Not working
        public static (Customer, Customer) CreateNewStoreCustomer(string name,
            [Optionally] string demographics,
            [Injected] Guid g1,
            [Injected] Guid g2,
            [Injected] Guid g3,
            [Injected] DateTime d)
        {
            var s = new Store(name, demographics, null, null, d, g1, 0, new List<BusinessEntityAddress>(), new List<BusinessEntityContact>(), g2, d);
            var c = new Customer(s, null, g3, d);
            return DisplayAndPersist(c);
        }

        public static (Customer, Customer) CreateCustomerFromStore(
            Store store, 
            [Injected] Guid guid,
            [Injected] DateTime dt)
        {
            return DisplayAndPersist(new Customer(store, null, guid, dt));
        }

        //TODO: Temporary exploration
        public static (Store, Store) CreateNewStoreOnly(
            string name,
            [Optionally] string demographics,
            [Injected] Guid guid1,
            [Injected] Guid guid2,
            [Injected] DateTime dt
            )
        {
            var store = new Store(name, demographics, null, null, dt, guid1, 0, new List<BusinessEntityAddress>(), new List<BusinessEntityContact>(), guid2, dt);
            return DisplayAndPersist(store);
        }

        public static IQueryable<Store> FindStoreOnlyByName(
           [Description("partial match")]string name,
           [Injected] IQueryable<Store> stores
           )
        {
            return stores.Where(s => s.Name.ToUpper().Contains(name.ToUpper()));
        }

        public static Customer RandomStore(
            [Injected] IQueryable<Customer> customers,
            [Injected] int random) {
            return Random(customers.Where(t => t.StoreID != null), random);
        }
        #endregion

        #region Individuals Menu

        [FinderAction]
        [MemberOrder(30)]
        [TableView(true)] //Table view == List View
        public static IQueryable<Customer> FindIndividualCustomerByName(
            
            [Optionally] string firstName, 
            string lastName, 
            [Injected] IQueryable<Person> persons,
            [Injected] IQueryable<Customer> customers) {

            IQueryable<Person> matchingPersons = PersonRepository.FindContactByName( firstName, lastName, persons);
            return from c in customers
                   from p in matchingPersons
                   where c.PersonID == p.BusinessEntityID
                   select c;
        }

        [FinderAction]
        [MemberOrder(50)]
        public static (Customer, Customer) CreateNewIndividualCustomer(
            string firstName, 
            string lastName, 
            [DataType(DataType.Password)] string initialPassword) {

            //var person = new Person(firstName,"",lastName, 0, false); //person.EmailPromotion = 0; person.NameStyle = false;
            //var (person2, _) = PersonFunctions.ChangePassword(person, null, initialPassword, null); 
            //var indv = new Customer(null, person2);
            //return (indv, indv);  //TODO: check that this will persist the associated Person as well as Customer
            return (null, null);
        }

        [FinderAction]
        [MemberOrder(70)]
        public static Customer RandomIndividual(
            [Injected] IQueryable<Customer> customers,
            [Injected] int random)
        {
            return Random(customers.Where(t => t.StoreID == null), random);
        }


        #endregion


        [TableView(false, "AccountNumber","Store","Person","SalesTerritory")]
        
        public static List<Customer> RandomCustomers(
            [Injected] IQueryable<Customer> customers,
            [Injected] int random1,
            [Injected] int random2)
        {
            var list = new List<Customer>();
            list.Add(RandomIndividual(customers, random1));
            list.Add(RandomStore(customers, random2));
            return list;
        }
    }
}