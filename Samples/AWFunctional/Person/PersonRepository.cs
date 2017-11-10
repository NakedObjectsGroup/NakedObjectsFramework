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
using NakedFunctions;
using System;

namespace AdventureWorksModel {
    [DisplayName("Contacts")]
    public class PersonRepository : AWAbstractFactoryAndRepository {

        #region FindContactByName

        
        [TableView(true, nameof(Person.AdditionalContactInfo))]
        public static QueryResultList FindContactByName([Optionally] string firstName, string lastName, IFunctionalContainer container) {
            return new QueryResultList(QueryContactByName(firstName, lastName, container));
        }

        internal static IQueryable<Person> QueryContactByName([Optionally] string firstName, string lastName, IFunctionalContainer container)
        {
            return container.Instances<Person>().
                Where(p => (firstName == null || p.FirstName.ToUpper().StartsWith(firstName.ToUpper())) &&
                      p.LastName.ToUpper().StartsWith(lastName.ToUpper())).
                OrderBy(p => p.LastName).ThenBy(p => p.FirstName);
        }
        #endregion

        public static QueryResultSingle RandomContact(IFunctionalContainer container) {
            return new QueryResultSingle(RandomPerson(container));
        }

        internal static Person RandomPerson(IFunctionalContainer container)
        {
            //TODO: Provide proper Function implementation of Random
            int random = new Random().Next(container.Instances<Person>().Count());
            //The OrderBy(...) doesn't do anything, but is a necessary precursor to using .Skip
            //which in turn is needed because LINQ to Entities doesn't support .ElementAt(x)
            return container.Instances<Person>().OrderBy(n => "").Skip(random).FirstOrDefault();
        }
        
        [TableView(true, nameof(Person.AdditionalContactInfo))]
        public static QueryResultList RandomContacts(IFunctionalContainer container) {
            object contact1 = RandomPerson(container);
            object contact2 = contact1;

            while (contact1 == contact2) {
                contact2 = RandomPerson(container);
            }

            return new QueryResultList(new[] {contact1, contact2}.AsQueryable());
        }

        #region ValidCountries

        /* This method is needed because the AW database insists that every address has a StateProvince (silly design!), yet
         * many Countries in the database have no associated StateProvince.
         */
        [TableView(true)] //Tableview == list view
        public static QueryResultList ValidCountries(IFunctionalContainer container) {
            return new QueryResultList(container.Instances<StateProvince>().
                Select(sp => sp.CountryRegion));
        }

        #endregion

        internal static IQueryable<Address> AddressesFor(IBusinessEntity entity, IFunctionalContainer container, string ofType = null)
        {
            int id = entity.BusinessEntityID;
            var baes = container.Instances<BusinessEntityAddress>();
            baes = baes.Where(bae => bae.BusinessEntityID == id);
            if (ofType != null)
            {
                baes = baes.Where(bae => bae.AddressType.Name == ofType);
            }
            return baes.Select(bae => bae.Address);
        }

        public static QueryResultList RecentAddresses(IFunctionalContainer container)
        {
            return new QueryResultList(container.Instances<Address>().OrderByDescending(a => a.ModifiedDate).Take(10).ToList());
        }

        public static QueryResultList RecentAddressLinks(IFunctionalContainer container)
        {
            return new QueryResultList(container.Instances<BusinessEntityAddress>().OrderByDescending(a => a.ModifiedDate).Take(10).ToList());
        }

        public static QueryResultList AllAddressTypes(IFunctionalContainer container)
        {
            return new QueryResultList(container.Instances<AddressType>());
        }
    }
}