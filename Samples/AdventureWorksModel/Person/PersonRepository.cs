// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NakedObjects;
using NakedObjects.Services;

namespace AdventureWorksModel {
    [DisplayName("Contacts")]
    public class PersonRepository : AbstractFactoryAndRepository {

        #region Injected Services
        #endregion

        #region FindContactByName

        [FinderAction]
        [TableView(true, nameof(Person.AdditionalContactInfo))]
        public IQueryable<Person> FindContactByName([Optionally] string firstName, string lastName) {
            IQueryable<Person> query = from obj in Instances<Person>()
                where (firstName == null || obj.FirstName.ToUpper().StartsWith(firstName.ToUpper())) &&
                      obj.LastName.ToUpper().StartsWith(lastName.ToUpper())
                orderby obj.LastName, obj.FirstName
                select obj;

            return query;
        }

        #endregion

        [FinderAction]
        [QueryOnly]
        public Person RandomContact() {
            return Random<Person>();
        }

        [FinderAction]
        [TableView(true, nameof(Person.AdditionalContactInfo))]
        public IQueryable<Person> RandomContacts() {
            Person contact1 = RandomContact();
            Person contact2 = contact1;

            while (contact1 == contact2) {
                contact2 = RandomContact();
            }

            return new[] {contact1, contact2}.AsQueryable();
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

        internal IQueryable<Address> AddressesFor(IBusinessEntity entity, string ofType = null) {
            int id = entity.BusinessEntityID;
            var baes = Container.Instances<BusinessEntityAddress>().Where(bae => bae.BusinessEntityID == id);
            if (ofType != null) {
                baes = baes.Where(bae => bae.AddressType.Name == ofType);
            }
            return baes.Select(bae => bae.Address);
        }


        public IList<Address> RecentAddresses()
        {
            return Container.Instances<Address>().OrderByDescending(a => a.ModifiedDate).Take(10).ToList();
        }

        public IList<BusinessEntityAddress> RecentAddressLinks()
        {
            return Container.Instances<BusinessEntityAddress>().OrderByDescending(a => a.ModifiedDate).Take(10).ToList();
        }


        public BusinessEntityAddress CreateNewBusinessEntityAddress()
        {
            BusinessEntityAddress obj = Container.NewTransientInstance<BusinessEntityAddress>();
            //set up any parameters
            //Container.Persist(ref obj);
            return obj;
        }



        public IQueryable<AddressType> AllAddressTypes()
        {
            return Container.Instances<AddressType>();
        }
    }
}