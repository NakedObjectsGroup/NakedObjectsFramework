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

namespace AdventureWorksModel {
    [DisplayName("Customers")]
    public class CustomerRepository : AbstractFactoryAndRepository {
        #region Injected Services

        #region Injected: ContactRepository

        public ContactRepository ContactRepository { set; protected get; }

        #endregion

        #endregion

        public static void Menu(IMenu menu) {
            menu.AddAction("FindCustomerByAccountNumber");
            menu.CreateSubMenu("Stores")
                .AddAction("FindStoreByName")
                //.AddAction("CreateNewStoreCustomer")  //Todo - to be written
                .AddAction("RandomStore");
            menu.CreateSubMenu("Individuals")
                .AddAction("FindIndividualCustomerByName")
                .AddAction("CreateNewIndividualCustomer")
                .AddAction("RandomIndividual");
            menu.AddAction("CustomerDashboard");
            menu.AddAction("ThrowDomainException");
            menu.AddRemainingNativeActions();
        }

        public void ThrowDomainException() {
            throw new DomainException("Foo");
        }

        [QueryOnly]
        public CustomerDashboard CustomerDashboard(string accountNumber) {
            Customer cust = FindCustomerByAccountNumber(accountNumber);
            var dash = Container.NewViewModel<CustomerDashboard>();
            dash.Root = cust;
            return dash;
        }

        #region FindCustomerByAccountNumber

        [FinderAction]
        [MemberOrder(10), QueryOnly]
        public Customer FindCustomerByAccountNumber([DefaultValue("AW")] string accountNumber) {
            IQueryable<Customer> query = from obj in Instances<Customer>()
                where obj.AccountNumber == accountNumber
                orderby obj.AccountNumber
                select obj;

            return SingleObjectWarnIfNoMatch(query);
        }

        public string ValidateFindCustomerByAccountNumber(string accountNumber) {
            var rb = new ReasonBuilder();
            rb.AppendOnCondition(!accountNumber.StartsWith("AW"), "Account number must start with AW");
            return rb.Reason;
        }

        #endregion

        #region Stores Menu

        [FinderAction]
        [MemberOrder(20)]
        [PageSize(2)]
        [TableView(true, "StoreName", "SalesPerson")] //Table view == List View
            public IQueryable<Customer> FindStoreByName(string name) {
                var customers = Instances<Customer>();
                var stores = Instances<Store>();
                return from c in customers
                       from s in stores
                where s.Name.ToUpper().Contains(name.ToUpper()) &&
                        c.StoreID == s.BusinessEntityID
                select c;
        }

        [FinderAction]
        [MemberOrder(60), QueryOnly]
        public Customer RandomStore() {
            var stores = StoreCustomers();
            int random = new Random().Next(stores.Count());
            //The OrderBy(...) doesn't do anything, but is a necessary precursor to using .Skip
            //which in turn is needed because LINQ to Entities doesn't support .ElementAt(x)
            return stores.OrderBy(n => "").Skip(random).FirstOrDefault();
        }

        private IQueryable<Customer> StoreCustomers() {
            var stores = Instances<Customer>().Where(t => t.StoreID != null);
            return stores;
        }

        #endregion

        #region Individuals Menu

        [FinderAction]
        [MemberOrder(30)]
        [TableView(true)] //Table view == List View
        public IQueryable<Customer> FindIndividualCustomerByName([Optionally] string firstName, string lastName) {
            IQueryable<Person> matchingPersons = ContactRepository.FindContactByName(firstName, lastName);
            return from c in Instances<Customer>()
                   from p in matchingPersons
                   where c.PersonID == p.BusinessEntityID
                   select c;
        }

        [FinderAction]
        [MemberOrder(50)]
        public Customer CreateNewIndividualCustomer(string firstName, string lastName, [DataType(DataType.Password)] string initialPassword) {
            var indv = NewTransientInstance<Customer>();
            var person = NewTransientInstance<Person>();
            person.FirstName = firstName;
            person.LastName = lastName;
            person.EmailPromotion = 0;
            person.NameStyle = false;
            person.ChangePassword(null, initialPassword, null);
            indv.Person = person;
            Persist(ref indv);
            return indv;
        }

        [FinderAction]
        [MemberOrder(70), QueryOnly]
        public Customer RandomIndividual() {
            var allIndividuals = IndividualCustomers();
            int random = new Random().Next(allIndividuals.Count());
            //The OrderBy(...) doesn't do anything, but is a necessary precursor to using .Skip
            //which in turn is needed because LINQ to Entities doesn't support .ElementAt(x)
            return allIndividuals.OrderBy(n => "").Skip(random).FirstOrDefault();
        }

        private IQueryable<Customer> IndividualCustomers() {
            var allIndividuals = Instances<Customer>().Where(t => t.StoreID == null);
            return allIndividuals;
        }

        #endregion

        public Customer RandomCustomer() {
            return Random<Customer>();
        }

        
        public IQueryable<Store> AllStores() {
            return Container.Instances<Store>();
        }
      
    }
}