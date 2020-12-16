// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedFunctions;
using NakedFunctions;

namespace AdventureWorksModel
{
            [Bounded]
    public record StateProvince :  IHasRowGuid, IHasModifiedDate
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
        public virtual int StateProvinceID { get; init; }

        public virtual string StateProvinceCode { get; init; }

        public virtual bool IsOnlyStateProvinceFlag { get; init; }

        public virtual string Name { get; init; }

        [NakedObjectsIgnore]
        public virtual string CountryRegionCode { get; init; }

        public virtual CountryRegion CountryRegion { get; init; }

        [NakedObjectsIgnore]
        public virtual int TerritoryID { get; init; }

        public virtual SalesTerritory SalesTerritory { get; set; }

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; init; }

        [MemberOrder(99)]
        
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; init; }
    }
    public static class StateProvinceFunctions
    {
          public static StateProvince Updating(StateProvince sp, [Injected] DateTime now)  => sp with { ModifiedDate = now };

    }
}