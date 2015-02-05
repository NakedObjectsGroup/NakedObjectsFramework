// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects;

namespace AdventureWorksModel {
    // ReSharper disable once PartialTypeWithSinglePart
    // ReSharper disable InconsistentNaming

    public partial class SalesPerson {
        #region Primitive Properties

        #region SalesPersonID (Int32)

        [MemberOrder(100)]
        public virtual int SalesPersonID { get; set; }

        #endregion

        #region SalesQuota (Decimal)

        [MemberOrder(110), Optionally]
        public virtual decimal? SalesQuota { get; set; }

        #endregion

        #region Bonus (Decimal)

        [MemberOrder(120)]
        public virtual decimal Bonus { get; set; }

        #endregion

        #region CommissionPct (Decimal)

        [MemberOrder(130)]
        public virtual decimal CommissionPct { get; set; }

        #endregion

        #region SalesYTD (Decimal)

        [MemberOrder(140)]
        public virtual decimal SalesYTD { get; set; }

        #endregion

        #region SalesLastYear (Decimal)

        [MemberOrder(150)]
        public virtual decimal SalesLastYear { get; set; }

        #endregion

        #region rowguid (Guid)

        [MemberOrder(160)]
        public virtual Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(170), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region Employee (Employee)

        [MemberOrder(180)]
        public virtual Employee Employee { get; set; }

        #endregion

        #region SalesOrderHeaders (Collection of SalesOrderHeader)

        private ICollection<SalesOrderHeader> _salesOrderHeaders = new List<SalesOrderHeader>();

        [MemberOrder(190), Disabled]
        public virtual ICollection<SalesOrderHeader> SalesOrderHeaders {
            get { return _salesOrderHeaders; }
            set { _salesOrderHeaders = value; }
        }

        #endregion

        #region SalesTerritory (SalesTerritory)

        [MemberOrder(200)]
        public virtual SalesTerritory SalesTerritory { get; set; }

        #endregion

        #region SalesPersonQuotaHistories (Collection of SalesPersonQuotaHistory)

        private ICollection<SalesPersonQuotaHistory> _salesPersonQuotaHistories = new List<SalesPersonQuotaHistory>();

        [MemberOrder(210), Disabled]
        public virtual ICollection<SalesPersonQuotaHistory> SalesPersonQuotaHistories {
            get { return _salesPersonQuotaHistories; }
            set { _salesPersonQuotaHistories = value; }
        }

        #endregion

        #region SalesTerritoryHistories (Collection of SalesTerritoryHistory)

        private ICollection<SalesTerritoryHistory> _salesTerritoryHistories = new List<SalesTerritoryHistory>();

        [MemberOrder(220), Disabled]
        public virtual ICollection<SalesTerritoryHistory> SalesTerritoryHistories {
            get { return _salesTerritoryHistories; }
            set { _salesTerritoryHistories = value; }
        }

        #endregion

        #region Stores (Collection of Store)

        private ICollection<Store> _stores = new List<Store>();

        [MemberOrder(230), Disabled]
        public virtual ICollection<Store> Stores {
            get { return _stores; }
            set { _stores = value; }
        }

        #endregion

        #endregion
    }
}