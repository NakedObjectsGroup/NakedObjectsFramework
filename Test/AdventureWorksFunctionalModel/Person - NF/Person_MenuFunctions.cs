// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using NakedFunctions;
using static NakedFunctions.Helpers;

namespace AdventureWorksModel {
    [Named("Contacts")]
    public static class Person_MenuFunctions {

        [TableView(true, nameof(Person.AdditionalContactInfo))]
        public static IQueryable<Person> FindContactByName(        
            [Optionally] string firstName, string lastName, IContainer container) =>
                container.Instances<Person>().Where(p => firstName == null || p.FirstName.ToUpper().StartsWith(firstName.ToUpper()) &&
                      p.LastName.ToUpper().StartsWith(lastName.ToUpper())).OrderBy(p => p.LastName).ThenBy(p => p.FirstName);
        
        public static Person RandomContact(IContainer container) {
            return Random<Person>(container);
        }

        [TableView(true, nameof(Person.AdditionalContactInfo))]
        public static IQueryable<Person> RandomContacts(IContainer container) {
            //Note: Cannot just call RandomContact twice as it would be with the same random number!
            var instances = container.Instances<Person>().OrderBy(n => "");
            IRandom random1 = container.GetService<IRandomSeedGenerator>().Random;
            IRandom random2 = random1.Next();
            Person p1 = instances.Skip(random1.ValueInRange(instances.Count())).FirstOrDefault();
            Person p2 = instances.Skip(random2.ValueInRange(instances.Count())).FirstOrDefault();
            return new[] {p1, p2}.AsQueryable();
        }

        #region ValidCountries

        /* This method is needed because the AW database insists that every address has a StateProvince (silly design!), yet
         * many Countries in the database have no associated StateProvince.
         */
        [TableView(true)] //Tableview == list view
        public static List<CountryRegion> ValidCountries(IContainer container) => container.Instances<StateProvince>().Select(sp => sp.CountryRegion).Distinct().ToList();

        #endregion

        internal static IQueryable<Address> AddressesFor(IBusinessEntity entity, IContainer container, string ofType = null) {
            int id = entity.BusinessEntityID;
            var baes = container.Instances< BusinessEntityAddress>().Where(bae => bae.BusinessEntityID == id);
            if (ofType != null) {
                baes = baes.Where(bae => bae.AddressType.Name == ofType);
            }
            return baes.Select(bae => bae.Address);
        }

        public static IList<Address> RecentAddresses(IContainer container) => container.Instances<Address>().OrderByDescending(a => a.ModifiedDate).Take(10).ToList();
 
        //public static IList<BusinessEntityAddress> RecentAddressLinks(IQueryable<BusinessEntityAddress> beAddresses)
        //{
        //    return beAddresses.OrderByDescending(a => a.ModifiedDate).Take(10).ToList();
        //}
    }
}