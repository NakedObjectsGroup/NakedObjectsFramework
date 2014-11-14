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
    public partial class ShipMethod {
        #region Primitive Properties

        #region ShipMethodID (Int32)

        [MemberOrder(100)]
        public virtual int ShipMethodID { get; set; }

        #endregion

        #region Name (String)

        [MemberOrder(110), StringLength(50)]
        public virtual string Name { get; set; }

        #endregion

        #region ShipBase (Decimal)

        [MemberOrder(120)]
        public virtual decimal ShipBase { get; set; }

        #endregion

        #region ShipRate (Decimal)

        [MemberOrder(130)]
        public virtual decimal ShipRate { get; set; }

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

        #region PurchaseOrderHeaders (Collection of PurchaseOrderHeader)

        private ICollection<PurchaseOrderHeader> _purchaseOrderHeaders = new List<PurchaseOrderHeader>();

        [MemberOrder(160), Disabled]
        public virtual ICollection<PurchaseOrderHeader> PurchaseOrderHeaders {
            get { return _purchaseOrderHeaders; }
            set { _purchaseOrderHeaders = value; }
        }

        #endregion

        #region SalesOrderHeaders (Collection of SalesOrderHeader)

        private ICollection<SalesOrderHeader> _salesOrderHeaders = new List<SalesOrderHeader>();

        [MemberOrder(170), Disabled]
        public virtual ICollection<SalesOrderHeader> SalesOrderHeaders {
            get { return _salesOrderHeaders; }
            set { _salesOrderHeaders = value; }
        }

        #endregion

        #endregion
    }
}