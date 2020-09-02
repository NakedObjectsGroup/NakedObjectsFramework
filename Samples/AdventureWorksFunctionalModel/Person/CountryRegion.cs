// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedFunctions;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("globe.png")]
    [Bounded]
    [Immutable]
    public class CountryRegion: IHasModifiedDate {

        public CountryRegion(string countryRegionCode, string name, DateTime modifiedDate)
        {
            CountryRegionCode = countryRegionCode;
            Name = name;
            ModifiedDate = modifiedDate;
        }

        public CountryRegion() { }

        public virtual string CountryRegionCode { get; set; }

        public virtual string Name { get; set; }

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }
    }

    public static class CountryRegionFunctions
    {
        public static string Title(this CountryRegion cr)
        {
            return cr.CreateTitle(cr.Name);
        }

        public static CountryRegion Updating(CountryRegion cr, [Injected] DateTime now)
        {
            return LifeCycleFunctions.UpdateModified(cr, now);

        }
    }
}