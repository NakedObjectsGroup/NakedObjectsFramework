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

namespace AdventureWorksModel
{
    [IconName("lookup.png")]
    [Immutable]
    [Bounded]
    public class StateProvince :  IHasRowGuid, IHasModifiedDate
    {
        //TODO: Extend ctor to include all properties
        public StateProvince(
            int stateProvinceID,
            string stateProvinceCode,
            bool isOnlyStateProvinceFlag,
            string name,
            string countryRegionCode,
            int territoryID,
            SalesTerritory salesTerritory,
            Guid rowguid,
            DateTime modifiedDate
            )
        {
            StateProvinceID = stateProvinceID;
            StateProvinceCode = stateProvinceCode;
            IsOnlyStateProvinceFlag = isOnlyStateProvinceFlag;
            Name = name;
            CountryRegionCode = countryRegionCode;
            TerritoryID = territoryID;
            SalesTerritory = salesTerritory;
            this.rowguid = rowguid;
            ModifiedDate = modifiedDate;
        }

        public StateProvince() { }

        [NakedObjectsIgnore]
        public virtual int StateProvinceID { get; set; }

        public virtual string StateProvinceCode { get; set; }

        public virtual bool IsOnlyStateProvinceFlag { get; set; }

        public virtual string Name { get; set; }

        [NakedObjectsIgnore]
        public virtual string CountryRegionCode { get; set; }

        public virtual CountryRegion CountryRegion { get; set; }

        [NakedObjectsIgnore]
        public virtual int TerritoryID { get; set; }

        public virtual SalesTerritory SalesTerritory { get; set; }

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; set; }

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }
    }
    public static class StateProvinceFunctions
    {
        public static string Title(this StateProvince sp)
        {
            return sp.CreateTitle(sp.Name);
        }
        public static StateProvince Updating(StateProvince sp, [Injected] DateTime now)
        {
            return LifeCycleFunctions.UpdateModified(sp, now);

        }
    }
}