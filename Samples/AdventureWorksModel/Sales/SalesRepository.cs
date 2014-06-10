// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

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

        [QueryOnly]
        public SalesPerson RandomSalesPerson() {
            return Random<SalesPerson>();
        }

        [Idempotent]
        public SalesPerson CreateNewSalesPerson(Employee employee) {
            var salesPerson = NewTransientInstance<SalesPerson>();
            salesPerson.Employee = employee;
            return salesPerson;
        }

        #region ListAccountsForSalesPerson

        [TableView(true)] //TableView == ListView
        public IQueryable<Store> ListAccountsForSalesPerson(SalesPerson sp) {
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