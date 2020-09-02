// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NakedFunctions;
using NakedObjects;
using static AdventureWorksModel.CommonFactoryAndRepositoryFunctions;

namespace AdventureWorksModel {
    [DisplayName("Contacts")]
    public static class PersonRepository {

        [TableView(true, nameof(Person.AdditionalContactInfo))]
        public static IQueryable<Person> FindContactByName(
            
            [Optionally] string firstName, 
            string lastName, [Injected]
        IQueryable<Person> persons) {
            return persons.Where(p => firstName == null || p.FirstName.ToUpper().StartsWith(firstName.ToUpper()) &&
                      p.LastName.ToUpper().StartsWith(lastName.ToUpper())).OrderBy(p => p.LastName).ThenBy(p => p.FirstName);
        }

        
        public static Person RandomContact(
            
            [Injected] IQueryable<Person> persons, 
            [Injected] int random) {
            return Random(persons, random);
        }

        [FinderAction]
        [TableView(true, nameof(Person.AdditionalContactInfo))]
        public static IQueryable<Person> RandomContacts(
            
            [Injected] IQueryable<Person> persons,
            [Injected] int random1, 
            [Injected] int random2) {
            Person contact1 = RandomContact(persons, random1);
            Person contact2 = RandomContact(persons, random2);
            return new[] {contact1, contact2}.AsQueryable();
        }

        #region ValidCountries

        /* This method is needed because the AW database insists that every address has a StateProvince (silly design!), yet
         * many Countries in the database have no associated StateProvince.
         */
        [TableView(true)] //Tableview == list view
        public static List<CountryRegion> ValidCountries(
            
            [Injected] IQueryable<StateProvince> sps) {
            return sps.Select(sp => sp.CountryRegion).Distinct().ToList();
        }

        #endregion

        internal static IQueryable<Address> AddressesFor(IBusinessEntity entity, [Injected] IQueryable<BusinessEntityAddress> allBaes, string ofType = null) {
            int id = entity.BusinessEntityID;
            var baes = allBaes.Where(bae => bae.BusinessEntityID == id);
            if (ofType != null) {
                baes = baes.Where(bae => bae.AddressType.Name == ofType);
            }
            return baes.Select(bae => bae.Address);
        }


        public static IList<Address> RecentAddresses(
             
            [Injected] IQueryable<Address> addresses)
        {
            return addresses.OrderByDescending(a => a.ModifiedDate).Take(10).ToList();
        }

        //public static IList<BusinessEntityAddress> RecentAddressLinks([Injected] IQueryable<BusinessEntityAddress> beAddresses)
        //{
        //    return beAddresses.OrderByDescending(a => a.ModifiedDate).Take(10).ToList();
        //}

    }
}