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

namespace AdventureWorksModel {
    [DisplayName("Sales")]
    public class SalesRepository : AbstractFactoryAndRepository {
        #region Injected Services

        #region Injected: ContactRepository

        public ContactRepository ContactRepository { set; protected get; }

        #endregion

        #endregion

        #region FindSalesPersonByName

        [FinderAction("WithPrefix")]
        [TableView(true, "SalesTerritory")]
        public IQueryable<SalesPerson> FindSalesPersonByName([Optionally] string firstName, string lastName) {
            IQueryable<Contact> matchingContacts = ContactRepository.FindContactByName(firstName, lastName);

            return from sp in Instances<SalesPerson>()
                from contact in matchingContacts
                where sp.Employee.ContactDetails.ContactID == contact.ContactID
                orderby sp.Employee.ContactDetails.LastName, sp.Employee.ContactDetails.FirstName
                select sp;
        }

        #endregion

        [FinderAction]
        [QueryOnly]
        public SalesPerson RandomSalesPerson() {
            return Random<SalesPerson>();
        }

        [FinderAction]
        [Idempotent]
        public SalesPerson CreateNewSalesPerson([ContributedAction("Sales")] Employee employee) {
            var salesPerson = NewTransientInstance<SalesPerson>();
            salesPerson.Employee = employee;
            return salesPerson;
        }

        #region ListAccountsForSalesPerson

        [TableView(true)] //TableView == ListView
        public IQueryable<Store> ListAccountsForSalesPerson([ContributedAction("Sales")] SalesPerson sp) {
            return from obj in Instances<Store>()
                where obj.SalesPerson.SalesPersonID == sp.SalesPersonID
                select obj;
        }

        [PageSize(20)]
        public IQueryable<SalesPerson> AutoComplete0ListAccountsForSalesPerson([MinLength(2)] string name) {
            return Container.Instances<SalesPerson>().Where(sp => sp.Employee.ContactDetails.LastName.ToUpper().StartsWith(name.ToUpper()));
        }

        #endregion
    }
}