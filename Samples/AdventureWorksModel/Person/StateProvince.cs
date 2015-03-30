// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("lookup.png")]
    [Immutable]
    [Bounded]
    public class StateProvince : AWDomainObject {
        #region StateProvinceID

        [NakedObjectsIgnore]
        public virtual int StateProvinceID { get; set; }

        #endregion

        #region StateProvinceCode

        public virtual string StateProvinceCode { get; set; }

        #endregion

        #region IsOnlyStateProvinceFlag

        public virtual bool IsOnlyStateProvinceFlag { get; set; }

        #endregion

        #region Name

        [Title]
        public virtual string Name { get; set; }

        #endregion

        #region CountryRegion

        [NakedObjectsIgnore]
        public virtual string CountryRegionCode { get; set; }

        public virtual CountryRegion CountryRegion { get; set; }

        #endregion

        #region SalesTerritory

        [NakedObjectsIgnore]
        public int TerritoryID { get; set; }

        public virtual SalesTerritory SalesTerritory { get; set; }

        #endregion

        #region Row Guid and Modified Date

        #region rowguid

        [NakedObjectsIgnore]
        public override Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion

        #endregion
    }
}