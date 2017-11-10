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
using NakedObjects.Services;
using NakedObjects.Menu;
using NakedFunctions;

namespace AdventureWorksModel {
    [DisplayName("Sales")]
    public class SalesRepository : AWAbstractFactoryAndRepository {

        #region FindSalesPersonByName

        
        [TableView(true, "SalesTerritory")]
        public static IQueryable<SalesPerson> FindSalesPersonByName([Optionally] string firstName, string lastName, IFunctionalContainer container) {
            IQueryable<Person> matchingPersons = PersonRepository.QueryContactByName(firstName, lastName, container);

            return from sp in container.Instances<SalesPerson>()
                from person in matchingPersons
                where sp.BusinessEntityID == person.BusinessEntityID
                orderby sp.EmployeeDetails.PersonDetails.LastName, sp.EmployeeDetails.PersonDetails.FirstName
                select sp;
        }

        #endregion

        
        [QueryOnly]
        public static QueryResultSingle RandomSalesPerson(IFunctionalContainer container) {
            return Random<SalesPerson>(container);
        }

        
        [Idempotent][Description("... from an existing Employee")]
        public static SalesPerson CreateNewSalesPerson([ContributedAction("Sales")] Employee employee) {
            var salesPerson = new SalesPerson();
            salesPerson.EmployeeDetails = employee;
            return salesPerson;
        }

        #region ListAccountsForSalesPerson

        [TableView(true)] //TableView == ListView
        public static IQueryable<Store> ListAccountsForSalesPerson([ContributedAction("Sales")] SalesPerson sp, IFunctionalContainer container) {
            return from obj in container.Instances<Store>()
                where obj.SalesPerson.BusinessEntityID == sp.BusinessEntityID
                select obj;
        }

        [PageSize(20)]
        public static IQueryable<SalesPerson> AutoComplete0ListAccountsForSalesPerson([MinLength(2)] string name, IQueryable<SalesPerson> persons) {
            return persons.Where(sp => sp.EmployeeDetails.PersonDetails.LastName.ToUpper().StartsWith(name.ToUpper()));
        }

        #endregion


        public static IQueryable<SalesTaxRate> SalesTaxRates(IQueryable<SalesTaxRate> rates)
        {
            return rates;
        }

        public static QueryResultSingle RandomSalesTaxRate(IFunctionalContainer container)
        {
            return Random<SalesTaxRate>(container);
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
        public static void Action1() { }
        public static void Action2() { }
        public static void Action3() { }
        public static void Action4() { }
        #endregion

    }
}