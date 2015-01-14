// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel {
    // ReSharper disable once PartialTypeWithSinglePart
    // ReSharper disable InconsistentNaming

    public partial class SalesTerritory {
        #region Primitive Properties

        #region TerritoryID (Int32)

        [MemberOrder(100)]
        public virtual int TerritoryID { get; set; }

        #endregion

        #region Name (String)

        [MemberOrder(110), StringLength(50)]
        public virtual string Name { get; set; }

        #endregion

        #region CountryRegionCode (String)

        [MemberOrder(120), StringLength(3)]
        public virtual string CountryRegionCode { get; set; }

        #endregion

        #region Group (String)

        [MemberOrder(130), StringLength(50)]
        public virtual string Group { get; set; }

        #endregion

        #region SalesYTD (Decimal)

        [MemberOrder(140)]
        public virtual decimal SalesYTD { get; set; }

        #endregion

        #region SalesLastYear (Decimal)

        [MemberOrder(150)]
        public virtual decimal SalesLastYear { get; set; }

        #endregion

        #region CostYTD (Decimal)

        [MemberOrder(160)]
        public virtual decimal CostYTD { get; set; }

        #endregion

        #region CostLastYear (Decimal)

        [MemberOrder(170)]
        public virtual decimal CostLastYear { get; set; }

        #endregion

        #region rowguid (Guid)

        [MemberOrder(180)]
        public virtual Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(190), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region StateProvinces (Collection of StateProvince)

        private ICollection<StateProvince> _stateProvinces = new List<StateProvince>();

        [MemberOrder(200), Disabled]
        public virtual ICollection<StateProvince> StateProvinces {
            get { return _stateProvinces; }
            set { _stateProvinces = value; }
        }

        #endregion

        #region Customers (Collection of Customer)

        private ICollection<Customer> _customers = new List<Customer>();

        [MemberOrder(210), Disabled]
        public virtual ICollection<Customer> Customers {
            get { return _customers; }
            set { _customers = value; }
        }

        #endregion

        #region SalesOrderHeaders (Collection of SalesOrderHeader)

        private ICollection<SalesOrderHeader> _salesOrderHeaders = new List<SalesOrderHeader>();

        [MemberOrder(220), Disabled]
        public virtual ICollection<SalesOrderHeader> SalesOrderHeaders {
            get { return _salesOrderHeaders; }
            set { _salesOrderHeaders = value; }
        }

        #endregion

        #region SalesPersons (Collection of SalesPerson)

        private ICollection<SalesPerson> _salesPersons = new List<SalesPerson>();

        [MemberOrder(230), Disabled]
        public virtual ICollection<SalesPerson> SalesPersons {
            get { return _salesPersons; }
            set { _salesPersons = value; }
        }

        #endregion

        #region SalesTerritoryHistories (Collection of SalesTerritoryHistory)

        private ICollection<SalesTerritoryHistory> _salesTerritoryHistories = new List<SalesTerritoryHistory>();

        [MemberOrder(240), Disabled]
        public virtual ICollection<SalesTerritoryHistory> SalesTerritoryHistories {
            get { return _salesTerritoryHistories; }
            set { _salesTerritoryHistories = value; }
        }

        #endregion

        #endregion
    }
}