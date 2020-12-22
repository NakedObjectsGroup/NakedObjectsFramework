// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using NakedFunctions;
using static AdventureWorksModel.Helpers;

namespace AdventureWorksModel {
    [Named("Persons")]
    public static class Person_MenuFunctions {

        [TableView(true, nameof(Person.AdditionalContactInfo))]
        public static IQueryable<Person> FindContactByName(        
            [Optionally] string firstName, string lastName, IContext context) =>
                context.Instances<Person>().Where(p => firstName == null || p.FirstName.ToUpper().StartsWith(firstName.ToUpper()) &&
                      p.LastName.ToUpper().StartsWith(lastName.ToUpper())).OrderBy(p => p.LastName).ThenBy(p => p.FirstName);
        
        public static Person RandomContact(IContext context) {
            return Random<Person>(context);
        }

        [TableView(true, nameof(Person.AdditionalContactInfo))]
        public static IQueryable<Person> RandomContacts(IContext context) {
            var instances = context.Instances<Person>().OrderBy(n => "");
            IRandom random1 = context.GetService<IRandomSeedGenerator>().Random;
            IRandom random2 = random1.Next();
            Person p1 = instances.Skip(random1.ValueInRange(instances.Count())).FirstOrDefault();
            Person p2 = instances.Skip(random2.ValueInRange(instances.Count())).FirstOrDefault();
            return new[] {p1, p2}.AsQueryable();
        }

        /* This method is needed because the AW database insists that every address has a StateProvince (silly design!), yet
         * many Countries in the database have no associated StateProvince.
         */
        [TableView(true)] //Tableview == list view
        public static List<CountryRegion> ValidCountries(IContext context) => context.Instances<StateProvince>().Select(sp => sp.CountryRegion).Distinct().ToList();

        internal static IQueryable<Address> AddressesFor(IBusinessEntity entity, IContext context, string ofType = null) {
            int id = entity.BusinessEntityID;
            var baes = context.Instances< BusinessEntityAddress>().Where(bae => bae.BusinessEntityID == id);
            if (ofType != null) {
                baes = baes.Where(bae => bae.AddressType.Name == ofType);
            }
            return baes.Select(bae => bae.Address);
        }

        public static IList<Address> RecentAddresses(IContext context) => context.Instances<Address>().OrderByDescending(a => a.ModifiedDate).Take(10).ToList();

        public static IList<BusinessEntityAddress> RecentAddressLinks(IContext context) =>
         context.Instances<BusinessEntityAddress>().OrderByDescending(a => a.ModifiedDate).Take(10).ToList();
    }
}