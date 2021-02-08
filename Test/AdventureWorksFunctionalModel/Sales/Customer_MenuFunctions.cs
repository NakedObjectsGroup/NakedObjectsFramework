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

        [PageSize(2), MemberOrder("Stores", 1)]
        [TableView(true, "StoreName", "SalesPerson")] //Table view == List View
            public static IQueryable<Customer> FindStoreByName(
            [DescribedAs("partial match")]string name, IContext context) =>
            from c in context.Instances<Customer>()
                       from s in context.Instances<Store>()
                where s.Name.ToUpper().Contains(name.ToUpper()) &&
                        c.StoreID == s.BusinessEntityID
                select c;

        [MemberOrder("Stores",2)]
        public static (Customer, IContext) CreateNewStoreCustomer(
            string name, IContext context)
        {
            var s = new Store() with { Name = name, rowguid = context.NewGuid(), BusinessEntityRowguid = context.NewGuid(), ModifiedDate = context.Now(), BusinessEntityModifiedDate = context.Now() };
            var c = new Customer() with { Store = s, CustomerRowguid = context.NewGuid(), CustomerModifiedDate = context.Now() };
            return (c, context.WithNew(c).WithNew(s));
        }

        [MemberOrder("Stores",3)]
        public static Customer RandomStore(IContext context) {
            var stores = context.Instances<Customer>().Where(t => t.StoreID != null).OrderBy(t => "");
            var random = context.RandomSeed().ValueInRange(stores.Count());
            return stores.Skip(random).FirstOrDefault();
        }

        #endregion

        #region Individuals Menu

        [MemberOrder("Individuals",1)]
        [TableView(true)] //Table view == List View
        public static IQueryable<Customer> FindIndividualCustomerByName(
            [Optionally] string firstName, string lastName, IContext context)
        {
           IQueryable<Person> matchingPersons = Person_MenuFunctions.FindPersonsByName(firstName, lastName, context);
            return from c in context.Instances<Customer>()
                   from p in matchingPersons
                   where c.PersonID == p.BusinessEntityID
                   select c;
        }

        [MemberOrder("Individuals",2)]
        public static (Customer, IContext) CreateNewIndividualCustomer(
            string firstName, 
            string lastName,
            IContext context) {
            var p = new Person() with {
                PersonType = "SC",
                FirstName = firstName,
                LastName = lastName,
                NameStyle = false,
                EmailPromotion = 0,
                rowguid = context.NewGuid(),
                BusinessEntityRowguid = context.NewGuid(),
                ModifiedDate = context.Now(),
                BusinessEntityModifiedDate = context.Now()
            };
            var c = new Customer() with { Person = p, CustomerRowguid = context.NewGuid(), CustomerModifiedDate = context.Now() };
            return (c, context.WithNew(c).WithNew(p));
        }

        [MemberOrder("Individuals",3)]
        public static Customer RandomIndividual(IContext context)
        {
            var indivs = context.Instances<Customer>().Where(t => t.PersonID != null).OrderBy(t => "");
            var random = context.RandomSeed().ValueInRange(indivs.Count());
            return indivs.Skip(random).FirstOrDefault();
        }

        [MemberOrder("Individuals", 4)]
        public static IQueryable<Customer> RecentIndividualCustomers(IContext context) =>
         context.Instances<Customer>().Where(t => t.PersonID != null).OrderByDescending(t => t.CustomerModifiedDate);


        #endregion

        public static IQueryable<Customer> ListCustomersForSalesTerritory(SalesTerritory territory, IContext context)
        {
            var id = territory.TerritoryID;
            return context.Instances<Customer>().Where(c => c.SalesTerritory.TerritoryID == id);
        }
        
    }
}