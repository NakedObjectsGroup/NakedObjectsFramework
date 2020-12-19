// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.



using System.Linq;
using System;
using System.Collections.Generic;
using NakedFunctions;
using static NakedFunctions.Helpers;

namespace AdventureWorksModel {
    public static class Customer_MenuFunctions {

       
        //public static CustomerDashboard CustomerDashboard(
        //    string accountNumber,
        //    IQueryable<Customer> customers) {
        //     var (cust, _) = FindCustomerByAccountNumber(accountNumber, customers);
        //    return new CustomerDashboard(cust);
        //}

        //#region FindCustomerByAccountNumber

        //[MemberOrder(10)]
        //public static (Customer, string) FindCustomerByAccountNumber(
        //    [DefaultValue("AW")] string accountNumber, 
        //    IQueryable<Customer> customers)
        //{
        //    IQueryable<Customer> query = from obj in customers
        //        where obj.AccountNumber == accountNumber
        //        orderby obj.AccountNumber
        //        select obj;
        //    return SingleObjectWarnIfNoMatch(query);
        //}

        //public static string ValidateFindCustomerByAccountNumber( string accountNumber) {
        //    return accountNumber.StartsWith("AW")? null : "Account number must start with AW";
        //}

        //Method exists to test auto-complete
        public static Customer FindCustomer([DescribedAs("Enter Account Number")] Customer customer)
        {
            return customer;
        }

        //[PageSize(10)]
        //public static IQueryable<Customer> AutoComplete0FindCustomer([Range(3,0)] string matching, IQueryable<Customer> customers)
        //{
        //    return customers.Where(c => c.AccountNumber.Contains(matching));
        //}


        #region Stores Menu

        [PageSize(2)]
        [TableView(true, "StoreName", "SalesPerson")] //Table view == List View
            public static IQueryable<Customer> FindStoreByName(
            [DescribedAs("partial match")]string name,
            IQueryable<Customer> customers,
            IQueryable<Store> stores
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
            Guid g1,
            Guid g2,
            Guid g3,
            [Injected] DateTime d)
        {
            throw new NotImplementedException();
            //var s = new Store(name, demographics, null, null, d, g1, 0, new List<BusinessEntityAddress>(), new List<BusinessEntityContact>(), g2, d);
            //var c = new Customer(s, null, g3, d);
            //return DisplayAndPersist(c);
        }

        public static (Customer, Customer) CreateCustomerFromStore(
            Store store, 
            [Injected] Guid guid,
            [Injected] DateTime dt)
        {

            throw new NotImplementedException();
            //var c = new Customer(store, null, guid, dt);
            //return (c, c);
        }

        //TODO: Temporary exploration
        public static (Store, Store) CreateNewStoreOnly(
            string name,
            [Optionally] string demographics,
            Guid guid1,
            Guid guid2,
            [Injected] DateTime dt
            )
        {

            throw new NotImplementedException();
            //var store = new Store(name, demographics, null, null, dt, guid1, 0, new List<BusinessEntityAddress>(), new List<BusinessEntityContact>(), guid2, dt);
            //return DisplayAndPersist(store);
        }

        public static IQueryable<Store> FindStoreOnlyByName(
           [DescribedAs("partial match")]string name,
           IQueryable<Store> stores
           )
        {
            return stores.Where(s => s.Name.ToUpper().Contains(name.ToUpper()));
        }

        public static Customer RandomStore(
            IQueryable<Customer> customers,
            [Injected] int random) {
            return Random(customers.Where(t => t.StoreID != null), random);
        }
        #endregion

        #region Individuals Menu

        [MemberOrder(30)]
        [TableView(true)] //Table view == List View
        public static IQueryable<Customer> FindIndividualCustomerByName(
            [Optionally] string firstName,
            string lastName,
            IQueryable<Person> persons,
            IQueryable<Customer> customers)
        {
            throw new NotImplementedException();

            //IQueryable<Person> matchingPersons = PersonRepository.FindContactByName( firstName, lastName, persons);
            //return from c in customers
            //       from p in matchingPersons
            //       where c.PersonID == p.BusinessEntityID
            //       select c;
        }

        [MemberOrder(50)]
        public static (Customer, Customer) CreateNewIndividualCustomer(
            string firstName, 
            string lastName, 
            [Password] string initialPassword) {

            //var person = new Person(firstName,"",lastName, 0, false); //person.EmailPromotion = 0; person.NameStyle = false;
            //var (person2, _) = PersonFunctions.ChangePassword(person, null, initialPassword, null); 
            //var indv = new Customer(null, person2);
            //return (indv, indv);  //TODO: check that this will persist the associated Person as well as Customer
            return (null, null);
        }

        [MemberOrder(70)]
        public static Customer RandomIndividual(
            IQueryable<Customer> customers,
            [Injected] int random)
        {
            return Random(customers.Where(t => t.StoreID == null), random);
        }


        #endregion


        [TableView(false, "AccountNumber","Store","Person","SalesTerritory")]
        
        public static List<Customer> RandomCustomers(
            IQueryable<Customer> customers,
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