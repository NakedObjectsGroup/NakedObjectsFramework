// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedFunctions;

namespace AW.Types {
        public record PurchaseOrderHeader: IHasModifiedDate  {

        #region ID

        [Hidden]
        public virtual int PurchaseOrderID { get; init; }

        #endregion

        #region Revision Number

        
        [MemberOrder(90)]
        public virtual byte RevisionNumber { get; init; }

        #endregion

        [Hidden]
        public virtual int ShipMethodID { get; init; }

        [MemberOrder(22)]
        public virtual ShipMethod ShipMethod { get; init; }

        #region Vendor
        [Hidden]
        public virtual int VendorID { get; init; }

        [MemberOrder(1)]
        public virtual Vendor Vendor { get; init; }

        #endregion

        #region Status

        private static readonly string[] statusLabels = {"Pending", "Approved", "Rejected", "Complete"};

        [Hidden]
        [MemberOrder(10)]
        public virtual byte Status { get; init; }

        [Named("Status")]
        [MemberOrder(1)]
        public virtual string StatusAsString {
            get { return statusLabels[Status - 1]; }
        }

        #endregion

        #region Dates

        //Title
        [Mask("d")]
        [MemberOrder(11)]
        public virtual DateTime OrderDate { get; init; }

        [Mask("d")]
        [MemberOrder(20)]
        public virtual DateTime? ShipDate { get; init; }

        #endregion

        #region Amounts

        [MemberOrder(31)]
        
        [Mask("C")]
        public virtual decimal SubTotal { get; init; }

        
        [MemberOrder(32)]
        [Mask("C")]
        public virtual decimal TaxAmt { get; init; }

        
        [MemberOrder(33)]
        [Mask("C")]
        public virtual decimal Freight { get; init; }

        
        [MemberOrder(34)]
        [Mask("C")]
        public virtual decimal TotalDue { get; init; }

        #endregion

        #region Order Placed By (Employee)
        [Hidden]
        public virtual int OrderPlacedByID { get; init; }

        [MemberOrder(12)]
        public virtual Employee OrderPlacedBy { get; init; }
        #endregion

        [MemberOrder(99)]
        [Versioned]
		public virtual DateTime ModifiedDate { get; init; }

        [RenderEagerly]
        [TableView(true, "OrderQty", "Product", "UnitPrice", "LineTotal")]
        public virtual ICollection<PurchaseOrderDetail> Details { get; init; }

        public override string ToString() => $"PO from {Vendor}, {OrderDate}";
    }
}