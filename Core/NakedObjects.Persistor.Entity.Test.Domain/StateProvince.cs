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
    public partial class StateProvince {
        #region Primitive Properties

        #region StateProvinceID (Int32)

        [MemberOrder(100)]
        public virtual int StateProvinceID { get; set; }

        #endregion

        #region StateProvinceCode (String)

        [MemberOrder(110), StringLength(3)]
        public virtual string StateProvinceCode { get; set; }

        #endregion

        #region IsOnlyStateProvinceFlag (Boolean)

        [MemberOrder(120)]
        public virtual bool IsOnlyStateProvinceFlag { get; set; }

        #endregion

        #region Name (String)

        [MemberOrder(130), StringLength(50)]
        public virtual string Name { get; set; }

        #endregion

        #region rowguid (Guid)

        [MemberOrder(140)]
        public virtual Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(150), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region Addresses (Collection of Address)

        private ICollection<Address> _addresses = new List<Address>();

        [MemberOrder(160), Disabled]
        public virtual ICollection<Address> Addresses {
            get { return _addresses; }
            set { _addresses = value; }
        }

        #endregion

        #region CountryRegion (CountryRegion)

        [MemberOrder(170)]
        public virtual CountryRegion CountryRegion { get; set; }

        #endregion

        #region SalesTaxRates (Collection of SalesTaxRate)

        private ICollection<SalesTaxRate> _salesTaxRates = new List<SalesTaxRate>();

        [MemberOrder(180), Disabled]
        public virtual ICollection<SalesTaxRate> SalesTaxRates {
            get { return _salesTaxRates; }
            set { _salesTaxRates = value; }
        }

        #endregion

        #region SalesTerritory (SalesTerritory)

        [MemberOrder(190)]
        public virtual SalesTerritory SalesTerritory { get; set; }

        #endregion

        #endregion
    }
}