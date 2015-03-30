// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("globe.png")]
    [Bounded]
    [Immutable]
    public class SalesTerritory : AWDomainObject {
        #region ID

        [NakedObjectsIgnore]
        public virtual int TerritoryID { get; set; }

        #endregion

        #region Name

        [Title]
        [MemberOrder(10)]
        public virtual string Name { get; set; }

        #endregion

        [MemberOrder(20)]
        public virtual string CountryRegionCode { get; set; }

        [MemberOrder(30)]
        public virtual string Group { get; set; }

        [MemberOrder(40)]
        [Mask("C")]
        public virtual decimal SalesYTD { get; set; }

        [MemberOrder(41)]
        [Mask("C")]
        public virtual decimal SalesLastYear { get; set; }

        [MemberOrder(42)]
        [Mask("C")]
        public virtual decimal CostYTD { get; set; }

        [MemberOrder(43)]
        [Mask("C")]
        public virtual decimal CostLastYear { get; set; }

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

        #region States/Provinces covered

        private ICollection<StateProvince> _StateProvince = new List<StateProvince>();

        [DisplayName("States/Provinces covered")]
        [TableView(true)] //Table View == List View
        public virtual ICollection<StateProvince> StateProvince {
            get { return _StateProvince; }
            set { _StateProvince = value; }
        }

        #endregion
    }
}