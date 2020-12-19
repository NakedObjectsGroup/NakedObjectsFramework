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

        [MemberOrder(10)]
        public static (Customer, IContainer) FindCustomerByAccountNumber(
            [DefaultValue("AW")] string accountNumber, IContainer container) =>
                                         (from obj in container.Instances<Customer>()
                                         where obj.AccountNumber == accountNumber
                                         orderby obj.AccountNumber
                                         select obj).SingleObjectWarnIfNoMatch(container);

        public static string ValidateFindCustomerByAccountNumber(string accountNumber)
        {
            return accountNumber.StartsWith("AW") ? null : "Account number must start with AW";
        }

        //Method exists to test auto-complete
        public static Customer FindCustomer([DescribedAs("Enter Account Number")] Customer customer)
        {
            return customer;
        }

        [PageSize(10)]
        public static IQueryable<Customer> AutoComplete0FindCustomer([Range(3, 0)] string matching, IContainer container) =>
            container.Instances<Customer>().Where(c => c.AccountNumber.Contains(matching));

        #region Stores Menu

        [PageSize(2)]
        [TableView(true, "StoreName", "SalesPerson")] //Table view == List View
            public static IQueryable<Customer> FindStoreByName(
            [DescribedAs("partial match")]string name, IContainer container) =>
            from c in container.Instances<Customer>()
                       from s in container.Instances<Store>()
                where s.Name.ToUpper().Contains(name.ToUpper()) &&
                        c.StoreID == s.BusinessEntityID
                select c;


        //TODO:  Not working
        public static (Customer, IContainer) CreateNewStoreCustomer(string name,
            [Optionally] string demographics, IContainer container)
        {
            throw new NotImplementedException();
            //var s = new Store(name, demographics, null, null, d, g1, 0, new List<BusinessEntityAddress>(), new List<BusinessEntityContact>(), g2, d);
            //var c = new Customer(s, null, g3, d);
            //return DisplayAndPersist(c);
        }

        public static (Customer, IContainer) CreateCustomerFromStore(
            Store store, IContainer container) =>
            DisplayAndSave(new Customer() with { Store = store, CustomerRowguid = container.NewGuid(), CustomerModifiedDate = container.Now() }, container);

        //TODO: Temporary exploration
        public static (Store, IContainer) CreateNewStoreOnly(
            string name, [Optionally] string demographics, IContainer container){ 

            throw new NotImplementedException();
            //var store = new Store(name, demographics, null, null, dt, guid1, 0, new List<BusinessEntityAddress>(), new List<BusinessEntityContact>(), guid2, dt);
            //return DisplayAndPersist(store);
        }

        public static IQueryable<Store> FindStoreOnlyByName(
            [DescribedAs("partial match")]string name, IContainer container) => 
            container.Instances<Store>().Where(s => s.Name.ToUpper().Contains(name.ToUpper()));

        public static Customer RandomStore(IContainer container) {
            var stores = container.Instances<Customer>().Where(t => t.StoreID != null);
            var random = container.RandomSeed().ValueInRange(stores.Count());
            return stores.Skip(random).FirstOrDefault();
        }

        #endregion

        #region Individuals Menu

        [MemberOrder(30)]
        [TableView(true)] //Table view == List View
        public static IQueryable<Customer> FindIndividualCustomerByName(
            [Optionally] string firstName, string lastName, IContainer container)
        {
           IQueryable<Person> matchingPersons = Person_MenuFunctions.FindContactByName(firstName, lastName, container);
            return from c in container.Instances<Customer>()
                   from p in matchingPersons
                   where c.PersonID == p.BusinessEntityID
                   select c;
        }

        [MemberOrder(50)]
        public static (Customer, IContainer) CreateNewIndividualCustomer(
            string firstName, 
            string lastName, 
            [Password] string initialPassword) {
            throw new NotImplementedException();
            //var person = new Person() with { FirstName = firstName, LastName = lastName }; //person.EmailPromotion = 0; person.NameStyle = false;
            //var (person2, _) = Person_Functions.ChangePassword(person, null, initialPassword, null);
            //var indv = new Customer(null, person2);
            //return (indv, indv);  //TODO: check that this will persist the associated Person as well as Customer
        }

        [MemberOrder(70)]
        public static Customer RandomIndividual(IContainer container) => Random<Customer>(container);

        #endregion


        [TableView(false, "AccountNumber","Store","Person","SalesTerritory")]        
        public static List<Customer> RandomCustomers(IContainer container)
        {
            var list = new List<Customer>();
            //Added this slightly odd way around for historical test compatibility only
            IRandom random = container.RandomSeed().Next();
            list.Add(Random<Customer>(container));
            list.Add(RandomStore(container));
            return list;
        }
    }
}