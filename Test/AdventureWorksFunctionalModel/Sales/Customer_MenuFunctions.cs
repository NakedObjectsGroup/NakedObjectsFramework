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
using AW.Types;


namespace AW.Functions {

    [Named("Customers")]
    public static class Customer_MenuFunctions {


        //public static CustomerDashboard CustomerDashboard(
        //    string accountNumber,
        //    IQueryable<Customer> customers) {
        //     var (cust, _) = FindCustomerByAccountNumber(accountNumber, customers);
        //    return new CustomerDashboard(cust);
        //}

        [MemberOrder(10)]
        public static Customer FindCustomerByAccountNumber(
            [DefaultValue("AW")][RegEx(@"^AW\d{8}$")] string accountNumber, IContext context) =>
            context.Instances<Customer>().Where(x => x.AccountNumber == accountNumber).FirstOrDefault();


        #region Stores Menu

        [PageSize(2), MemberOrder(1, "Stores")]
        [TableView(true, "StoreName", "SalesPerson")] //Table view == List View
            public static IQueryable<Customer> FindStoreByName(
            [DescribedAs("partial match")]string name, IContext context) =>
            from c in context.Instances<Customer>()
                       from s in context.Instances<Store>()
                where s.Name.ToUpper().Contains(name.ToUpper()) &&
                        c.StoreID == s.BusinessEntityID
                select c;


        //TODO:  Not working
        [MemberOrder(2,  "Stores")]
        public static (Customer, IContext) CreateNewStoreCustomer(string name,
            [Optionally] string demographics, IContext context)
        {
            throw new NotImplementedException();
            //var s = new Store(name, demographics, null, null, d, g1, 0, new List<BusinessEntityAddress>(), new List<BusinessEntityContact>(), g2, d);
            //var c = new Customer(s, null, g3, d);
            //return DisplayAndPersist(c);
        }

        [MemberOrder(3, "Stores")]
        public static (Customer, IContext) CreateCustomerFromStore(
            Store store, IContext context) =>
            context.SaveAndDisplay(new Customer() with { Store = store, CustomerRowguid = context.NewGuid(), CustomerModifiedDate = context.Now() });

        //TODO: Temporary exploration
        [MemberOrder(4, "Stores")]
        public static (Store, IContext) CreateNewStoreOnly(
            string name, [Optionally] string demographics, IContext context){ 

            throw new NotImplementedException();
            //var store = new Store(name, demographics, null, null, dt, guid1, 0, new List<BusinessEntityAddress>(), new List<BusinessEntityContact>(), guid2, dt);
            //return DisplayAndPersist(store);
        }

        [MemberOrder(5, "Stores")]
        public static IQueryable<Store> FindStoreOnlyByName(
            [DescribedAs("partial match")]string name, IContext context) => 
            context.Instances<Store>().Where(s => s.Name.ToUpper().Contains(name.ToUpper()));

        [MemberOrder(6, "Stores")]
        public static Customer RandomStore(IContext context) {
            var stores = context.Instances<Customer>().Where(t => t.StoreID != null).OrderBy(t => "");
            var random = context.RandomSeed().ValueInRange(stores.Count());
            return stores.Skip(random).FirstOrDefault();
        }

        #endregion

        #region Individuals Menu

        [MemberOrder(30)]
        [TableView(true)] //Table view == List View
        public static IQueryable<Customer> FindIndividualCustomerByName(
            [Optionally] string firstName, string lastName, IContext context)
        {
           IQueryable<Person> matchingPersons = Person_MenuFunctions.FindContactByName(firstName, lastName, context);
            return from c in context.Instances<Customer>()
                   from p in matchingPersons
                   where c.PersonID == p.BusinessEntityID
                   select c;
        }

        [MemberOrder(50)]
        public static (Customer, IContext) CreateNewIndividualCustomer(
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
        public static Customer RandomIndividual(IContext context)
        {
            var indivs = context.Instances<Customer>().Where(t => t.PersonID != null).OrderBy(t => "");
            var random = context.RandomSeed().ValueInRange(indivs.Count());
            return indivs.Skip(random).FirstOrDefault();
        }

        #endregion

        public static IQueryable<Customer> ListCustomersForSalesTerritory(SalesTerritory territory, IContext context)
        {
            var id = territory.TerritoryID;
            return context.Instances<Customer>().Where(c => c.SalesTerritory.TerritoryID == id);
        }
        
    }
}