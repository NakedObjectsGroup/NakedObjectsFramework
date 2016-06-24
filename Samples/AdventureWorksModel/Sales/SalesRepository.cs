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

namespace AdventureWorksModel {
    [DisplayName("Sales")]
    public class SalesRepository : AbstractFactoryAndRepository {
        #region Injected Services
        public PersonRepository ContactRepository { set; protected get; }
        #endregion

        #region FindSalesPersonByName

        [FinderAction]
        [TableView(true, "SalesTerritory")]
        public IQueryable<SalesPerson> FindSalesPersonByName([Optionally] string firstName, string lastName) {
            IQueryable<Person> matchingPersons = ContactRepository.FindContactByName(firstName, lastName);

            return from sp in Instances<SalesPerson>()
                from person in matchingPersons
                where sp.BusinessEntityID == person.BusinessEntityID
                orderby sp.EmployeeDetails.PersonDetails.LastName, sp.EmployeeDetails.PersonDetails.FirstName
                select sp;
        }

        #endregion

        [FinderAction]
        [QueryOnly]
        public SalesPerson RandomSalesPerson() {
            return Random<SalesPerson>();
        }

        [FinderAction]
        [Idempotent][Description("... from an existing Employee")]
        public SalesPerson CreateNewSalesPerson([ContributedAction("Sales"), FindMenu] Employee employee) {
            var salesPerson = NewTransientInstance<SalesPerson>();
            salesPerson.EmployeeDetails = employee;
            return salesPerson;
        }

        #region ListAccountsForSalesPerson

        [TableView(true)] //TableView == ListView
        public IQueryable<Store> ListAccountsForSalesPerson([ContributedAction("Sales")] SalesPerson sp) {
            return from obj in Instances<Store>()
                where obj.SalesPerson.BusinessEntityID == sp.BusinessEntityID
                select obj;
        }

        [PageSize(20)]
        public IQueryable<SalesPerson> AutoComplete0ListAccountsForSalesPerson([MinLength(2)] string name) {
            return Container.Instances<SalesPerson>().Where(sp => sp.EmployeeDetails.PersonDetails.LastName.ToUpper().StartsWith(name.ToUpper()));
        }

        #endregion


        public IQueryable<SalesTaxRate> SalesTaxRates()
        {
            return Container.Instances<SalesTaxRate>();
        }

        public SalesTaxRate RandomSalesTaxRate()
        {
            return Random<SalesTaxRate>();
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
        public void Action1() { }
        public void Action2() { }
        public void Action3() { }
        public void Action4() { }
        #endregion

    }
}