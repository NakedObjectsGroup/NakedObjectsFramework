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

    public partial class PurchaseOrderHeader {
        #region Primitive Properties

        #region PurchaseOrderID (Int32)

        [MemberOrder(100)]
        public virtual int PurchaseOrderID { get; set; }

        #endregion

        #region RevisionNumber (Byte)

        [MemberOrder(110)]
        public virtual byte RevisionNumber { get; set; }

        #endregion

        #region Status (Byte)

        [MemberOrder(120)]
        public virtual byte Status { get; set; }

        #endregion

        #region OrderDate (DateTime)

        [MemberOrder(130), Mask("d")]
        public virtual DateTime OrderDate { get; set; }

        #endregion

        #region ShipDate (DateTime)

        [MemberOrder(140), Optionally, Mask("d")]
        public virtual DateTime? ShipDate { get; set; }

        #endregion

        #region SubTotal (Decimal)

        [MemberOrder(150)]
        public virtual decimal SubTotal { get; set; }

        #endregion

        #region TaxAmt (Decimal)

        [MemberOrder(160)]
        public virtual decimal TaxAmt { get; set; }

        #endregion

        #region Freight (Decimal)

        [MemberOrder(170)]
        public virtual decimal Freight { get; set; }

        #endregion

        #region TotalDue (Decimal)

        [MemberOrder(180)]
        public virtual decimal TotalDue { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(190), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region Employee (Employee)

        [MemberOrder(200)]
        public virtual Employee Employee { get; set; }

        #endregion

        #region PurchaseOrderDetails (Collection of PurchaseOrderDetail)

        private ICollection<PurchaseOrderDetail> _purchaseOrderDetails = new List<PurchaseOrderDetail>();

        [MemberOrder(210), Disabled]
        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails {
            get { return _purchaseOrderDetails; }
            set { _purchaseOrderDetails = value; }
        }

        #endregion

        #region ShipMethod (ShipMethod)

        [MemberOrder(220)]
        public virtual ShipMethod ShipMethod { get; set; }

        #endregion

        #region Vendor (Vendor)

        [MemberOrder(230)]
        public virtual Vendor Vendor { get; set; }

        #endregion

        #endregion
    }
}