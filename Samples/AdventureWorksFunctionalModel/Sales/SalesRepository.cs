// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedFunctions;
using NakedObjects;
using NakedObjects.Services;
using NakedObjects.Menu;
using static AdventureWorksModel.CommonFactoryAndRepositoryFunctions;

namespace AdventureWorksModel {
    [DisplayName("Sales")]
    public static class SalesRepository  {

        #region FindSalesPersonByName

        [FinderAction]
        [TableView(true, "SalesTerritory")]
        public static IQueryable<SalesPerson> FindSalesPersonByName(
            
            [Optionally] string firstName,
            string lastName, 
            [Injected] IQueryable<Person> persons,
            [Injected] IQueryable<SalesPerson> sps) {
            IQueryable<Person> matchingPersons = PersonRepository.FindContactByName( firstName, lastName, persons);
            return from sp in sps
                from person in matchingPersons
                where sp.BusinessEntityID == person.BusinessEntityID
                orderby sp.EmployeeDetails.PersonDetails.LastName, sp.EmployeeDetails.PersonDetails.FirstName
                select sp;
        }

        #endregion

       
        public static SalesPerson RandomSalesPerson(
             [Injected] IQueryable<SalesPerson> sps,
            [Injected] int random) {
            return Random(sps, random);
        }

        [Idempotent][Description("... from an existing Employee")]
        public static SalesPerson CreateNewSalesPerson([ContributedAction("Sales"), FindMenu] Employee employee) {
            //TODO:
            //var salesPerson = NewTransientInstance<SalesPerson>();
            //salesPerson.EmployeeDetails = employee;
            //return salesPerson;
            return null;
        }

        #region ListAccountsForSalesPerson

        [TableView(true)] //TableView == ListView
        public static IQueryable<Store> ListAccountsForSalesPerson(
            [ContributedAction("Sales")] SalesPerson sp,
            [Injected] IQueryable<Store> stores
            ) {
            return from obj in stores
                where obj.SalesPerson.BusinessEntityID == sp.BusinessEntityID
                select obj;
        }

        [PageSize(20)]
        public static IQueryable<SalesPerson> AutoComplete0ListAccountsForSalesPerson(
            [MinLength(2)] string name,
            [Injected] IQueryable<SalesPerson> sps
            ) {
            return sps.Where(sp => sp.EmployeeDetails.PersonDetails.LastName.ToUpper().StartsWith(name.ToUpper()));
        }

        #endregion


        public static IQueryable<SalesTaxRate> SalesTaxRates(
            [Injected] IQueryable<SalesTaxRate> strs)
        {
            return strs;
        }

        public static SalesTaxRate RandomSalesTaxRate(
            [Injected] IQueryable<SalesTaxRate> strs,
            [Injected] int random
            )
        {
            return Random(strs, random);
        }

        #region Sub-menu hierarchy for testing only

        public static void Menu(IMenu menu)
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