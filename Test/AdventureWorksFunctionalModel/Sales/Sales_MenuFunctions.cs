// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.



using System.Linq;
using NakedFunctions;
using AW.Types;
using NakedFramework;
using static AW.Helpers;

namespace AW.Functions {
    [Named("Sales")]
    public static class Sales_MenuFunctions  {

        #region FindSalesPersonByName
        [TableView(true, "SalesTerritory")]
        public static IQueryable<SalesPerson> FindSalesPersonByName(      
            [Optionally] string firstName, string lastName, IContext context) {
            IQueryable<Person> matchingPersons = Person_MenuFunctions.FindContactByName( firstName, lastName, context);
            return from sp in context.Instances<SalesPerson>()
                from person in matchingPersons
                where sp.BusinessEntityID == person.BusinessEntityID
                orderby sp.EmployeeDetails.PersonDetails.LastName, sp.EmployeeDetails.PersonDetails.FirstName
                select sp;
        }

        #endregion

       
        public static SalesPerson RandomSalesPerson(IContext context) => Random<SalesPerson>(context);

        [MemberOrder(1, "Sales")]
        [DescribedAs("... from an existing Employee")]
        public static SalesPerson CreateNewSalesPerson( Employee employee) {
            //TODO:
            //var salesPerson = NewTransientInstance<SalesPerson>();
            //salesPerson.EmployeeDetails = employee;
            //return salesPerson;
            return null;
        }

        #region ListAccountsForSalesPerson

        [TableView(true),MemberOrder(1, "Sales")] //TableView == ListView
        public static IQueryable<Store> ListAccountsForSalesPerson(
            SalesPerson sp,
            IQueryable<Store> stores
            ) {
            return from obj in stores
                where obj.SalesPerson.BusinessEntityID == sp.BusinessEntityID
                select obj;
        }

        [PageSize(20)]
        public static IQueryable<SalesPerson> AutoComplete0ListAccountsForSalesPerson(
            [Length(2)] string name,
            IQueryable<SalesPerson> sps
            ) {
            return sps.Where(sp => sp.EmployeeDetails.PersonDetails.LastName.ToUpper().StartsWith(name.ToUpper()));
        }

        #endregion

        public static SalesTaxRate RandomSalesTaxRate(IContext context) => Random<SalesTaxRate>(context);

        #region Sub-menu hierarchy for testing only

        internal static void Menu(IMenu menu)
        {
            menu.AddAction(nameof(CreateNewSalesPerson));
            menu.AddAction(nameof(FindSalesPersonByName));
            menu.AddAction(nameof(ListAccountsForSalesPerson));
            menu.AddAction(nameof(RandomSalesPerson));
            menu.AddAction(nameof(SalesTaxRates));
            menu.AddAction(nameof(RandomSalesTaxRate));
            menu.CreateSubMenu("Sub Menu")
                .AddAction(nameof(Action1))
                .CreateSubMenu("Level 2 sub menu")
                .AddAction(nameof(Action2))
                .CreateSubMenu("Level 3 sub menu")
                .AddRemainingNativeActions();
        }

        public static SalesTaxRate Action1() {return null; }
        public static SalesTaxRate Action2() { return null; }
        public static SalesTaxRate Action3() { return null; }
        public static SalesTaxRate Action4() { return null; }
        #endregion

    }
}