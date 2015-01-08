// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NakedObjects;
using NakedObjects.Services;

namespace AdventureWorksModel {
    [DisplayName("Contacts")]
    public class ContactRepository : AbstractFactoryAndRepository {
        #region Injected Services

        // This region should contain properties to hold references to any services required by the
        // object.  Use the 'injs' shortcut to add a new service.

        #endregion


        #region FindContactByName
        [FinderAction]
        [TableView(true,"Phone", "EmailAddress", "AdditionalContactInfo")]
        public IQueryable<Contact> FindContactByName([Optionally] string firstName, string lastName) {
            IQueryable<Contact> query = from obj in Instances<Contact>()
                                        where (firstName == null || obj.FirstName.ToUpper().StartsWith(firstName.ToUpper())) &&
                                              obj.LastName.ToUpper().StartsWith(lastName.ToUpper())
                                        orderby obj.LastName, obj.FirstName
                                        select obj;

            return query;
        }

        #endregion

        [FinderAction]
        [QueryOnly]
        public Contact RandomContact() {
            return Random<Contact>();
        }

        [FinderAction]
         [TableView(true, "Phone", "EmailAddress", "AdditionalContactInfo")]
        public IQueryable<Contact> RandomContacts() {
            Contact contact1 = RandomContact();
            Contact contact2 = contact1;

            while (contact1 == contact2) {
                contact2 = RandomContact();
            }

            return new []{contact1, contact2}.AsQueryable();
        }

        #region ValidCountries

        /* This method is needed because the AW database insists that every address has a StateProvince (silly design!), yet
         * many Countries in the database have no associated StateProvince.
         */
        [FinderAction]
        [QueryOnly]
        [TableView(true)] //Tableview == list view
        public List<CountryRegion> ValidCountries() {
            IQueryable<CountryRegion> query = from state in Instances<StateProvince>()
                                              select state.CountryRegion;

            return query.Distinct().ToList();
        }

        #endregion
    }
}