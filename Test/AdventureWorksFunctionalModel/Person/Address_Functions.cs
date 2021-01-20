// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using NakedFunctions;
using AW.Types;
using System;


namespace AW.Functions {
  
    public static class Address_Functions
    {

        //public static string Validate(this Address a, CountryRegion countryRegion, StateProvince stateProvince, IQueryable<StateProvince> allProvinces)
        //=> StateProvincesForCountry(countryRegion, allProvinces).Contains(stateProvince) ? null : "Invalid region";

        [Edit]
        public static (Address, IContext) EditStateProvince(this Address a,
            CountryRegion countryRegion, StateProvince stateProvince, IContext context) =>
                context.SaveAndDisplay(a with {StateProvince = stateProvince });

        public static IList<CountryRegion> Choices1EditStateProvince(this Address a, IContext context) =>
                context.Instances<CountryRegion>().ToArray();

        public static IList<StateProvince> Choices2EditStateProvince(this Address a, 
            CountryRegion countryRegion, IContext context)=> 
                countryRegion != null ? StateProvincesForCountry(countryRegion, context) 
                : new StateProvince[] { };

        internal static StateProvince[] StateProvincesForCountry(this CountryRegion country,
            IContext context) => 
            context.Instances<StateProvince>().Where(p => p.CountryRegion.CountryRegionCode == country.CountryRegionCode).OrderBy(p => p.Name).ToArray();

    }
}