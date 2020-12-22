// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedFunctions;
using static AdventureWorksModel.Helpers;


namespace AdventureWorksModel {
  
    public static class Address_Functions
    {
        #region LifeCycle methods
        public static Address Updating(Address x, [Injected] DateTime now) => x with { ModifiedDate = now };

        public static Address Persisting(Address x, [Injected] Guid guid, [Injected] DateTime now) => x with { rowguid = guid, ModifiedDate = now };

        //Any object or list returned by Persisted (or Updated), is not for display but to be persisted/updated
        //themselves (equivalent to second Tuple value returned from an Action).
        public static BusinessEntityAddress Persisted(Address a, [Injected] Guid guid, [Injected] DateTime now)
            =>  new BusinessEntityAddress() with { AddressID = a.AddressForID, AddressTypeID = a.AddressTypeID, BusinessEntityID = a.AddressForID, rowguid = guid, ModifiedDate = now };
        #endregion

        #region Property-associated functions
        public static string Validate(this Address a, CountryRegion countryRegion, StateProvince stateProvince, IQueryable<StateProvince> allProvinces)
        => StateProvincesForCountry(countryRegion, allProvinces).Contains(stateProvince) ? null : "Invalid region";

        public static IList<StateProvince> ChoicesStateProvince(this Address a, CountryRegion countryRegion, IQueryable<StateProvince> allProvinces)
        => countryRegion != null ? StateProvincesForCountry(countryRegion, allProvinces) : new List<StateProvince>();

        private static IList<StateProvince> StateProvincesForCountry(this CountryRegion country, IQueryable<StateProvince> provinces)
        => provinces.Where(p => p.CountryRegion.CountryRegionCode == country.CountryRegionCode).OrderBy(p => p.Name).ToList();
        #endregion 

    }
}