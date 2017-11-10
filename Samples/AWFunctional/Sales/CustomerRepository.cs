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
using NakedObjects.Services;
using System;
using System.Collections.Generic;
using NakedFunctions;

namespace AdventureWorksModel {
    [DisplayName("Customers")]
    public class CustomerRepository : AWAbstractFactoryAndRepository {

        public static void Menu(IMenu menu) {
            menu.AddAction(nameof(FindCustomerByAccountNumber));
            menu.CreateSubMenu("Stores")
                .AddAction(nameof(FindStoreByName))
                .AddAction(nameof(CreateNewStoreCustomer))
                .AddAction(nameof(RandomStore));
            menu.CreateSubMenu("Individuals")
                .AddAction(nameof(FindIndividualCustomerByName))
                .AddAction(nameof(CreateNewIndividualCustomer))
                .AddAction(nameof(RandomIndividual));
            menu.AddAction(nameof(CustomerDashboard));
            menu.AddAction(nameof(ThrowDomainException));
            menu.AddAction(nameof(FindCustomer));
            menu.AddRemainingNativeActions();
        }

        public static void ThrowDomainException() {
            throw new DomainException("Foo");
        }

        [QueryOnly]
        public static CustomerDashboard CustomerDashboard(
            string accountNumber,
            IFunctionalContainer container) {
            Customer cust = QueryCustomerByAccountNumber(accountNumber, container).FirstOrDefault();
            var dash = ViewModelHelper.NewViewModel<CustomerDashboard>(container);
            dash.Root = cust;
            return dash;
        }

        #region FindCustomerByAccountNumber

        [MemberOrder(10), QueryOnly]
        public static QueryResultSingle FindCustomerByAccountNumber([DefaultValue("AW")] string accountNumber, IFunctionalContainer container) {
            return SingleObjectWarnIfNoMatch(QueryCustomerByAccountNumber(accountNumber, container));
        }

        internal static IQueryable<Customer> QueryCustomerByAccountNumber(string accountNumber, IFunctionalContainer container)
        {
            return from obj in container.Instances<Customer>()
                                         where obj.AccountNumber == accountNumber
                                         orderby obj.AccountNumber
                                         select obj;
        }

        public static string ValidateFindCustomerByAccountNumber(string accountNumber) {
            var rb = new ReasonBuilder();
            rb.AppendOnCondition(!accountNumber.StartsWith("AW"), "Account number must start with AW");
            return rb.Reason;
        }

        //Method exists to test auto-complete
        public static Customer FindCustomer([Description("Enter Account Number")] Customer customer)
        {
            return customer;
        }

        [PageSize(10)]
        public static IQueryable<Customer> AutoComplete0FindCustomer([MinLength(3)] string matching, IQueryable<Customer> customers)
        {
            return customers.Where(c => c.AccountNumber.Contains(matching));
        }
        #endregion

        #region Stores Menu

        [PageSize(2)]
        [TableView(true, "StoreName", "SalesPerson")] //Table view == List View
            public static IQueryable<Customer> FindStoreByName([Description("partial match")]string name, IFunctionalContainer container) {
                var customers = container.Instances<Customer>();
                var stores = container.Instances<Store>();
                return from c in customers
                       from s in stores
                where s.Name.ToUpper().Contains(name.ToUpper()) &&
                        c.StoreID == s.BusinessEntityID
                select c;
        }

        public static Customer CreateNewStoreCustomer(string name) {
            var store = new Store();
            store.Name = name;
            Persist(ref store);
            var cust = new Customer();
            cust.Store = store;
            Persist(ref cust);
            return cust;
        }

        [FinderAction, QueryOnly]
        public static Customer RandomStore(IFunctionalContainer container) {
            var stores = StoreCustomers(container);
            int random = new Random().Next(stores.Count());
            //The OrderBy(...) doesn't do anything, but is a necessary precursor to using .Skip
            //which in turn is needed because LINQ to Entities doesn't support .ElementAt(x)
            return stores.OrderBy(n => "").Skip(random).FirstOrDefault();
        }

        private static IQueryable<Customer> StoreCustomers(IFunctionalContainer container) {
            var stores = container.Instances<Customer>().Where(t => t.StoreID != null);
            return stores;
        }

        #endregion

        #region Individuals Menu

        [MemberOrder(30)]
        [TableView(true)] //Table view == List View
        public static IQueryable<Customer> FindIndividualCustomerByName([Optionally] string firstName, string lastName, IFunctionalContainer container) {
            IQueryable<Person> matchingPersons = PersonRepository.QueryContactByName(firstName, lastName, container);
            return from c in container.Instances<Customer>()
                   from p in matchingPersons
                   where c.PersonID == p.BusinessEntityID
                   select c;
        }

        [MemberOrder(50)]
        public static Customer CreateNewIndividualCustomer(string firstName, string lastName, [DataType(DataType.Password)] string initialPassword) {
            var indv = new Customer();
            var person = new Person();
            person.FirstName = firstName;
            person.LastName = lastName;
            person.EmailPromotion = 0;
            person.NameStyle = false;
            person.ChangePassword(null, initialPassword, null);
            indv.Person = person;
            Persist(ref indv);
            return indv;
        }

        [MemberOrder(70), QueryOnly]
        public static Customer RandomIndividual(IFunctionalContainer container) {
            var allIndividuals = IndividualCustomers(container);
            int random = new Random().Next(allIndividuals.Count());
            //The OrderBy(...) doesn't do anything, but is a necessary precursor to using .Skip
            //which in turn is needed because LINQ to Entities doesn't support .ElementAt(x)
            return allIndividuals.OrderBy(n => "").Skip(random).FirstOrDefault();
        }

        private static IQueryable<Customer> IndividualCustomers(IFunctionalContainer container) {
            var allIndividuals = container.Instances<Customer>().Where(t => t.StoreID == null);
            return allIndividuals;
        }

        #endregion


        [TableView(false, "AccountNumber","Store","Person","SalesTerritory")]
        [QueryOnly]
        public static List<Customer> RandomCustomers(IFunctionalContainer container)
        {
            var list = new List<Customer>();
            list.Add(RandomIndividual(container));
            list.Add(RandomStore(container));
            return list;
        }
    }
}