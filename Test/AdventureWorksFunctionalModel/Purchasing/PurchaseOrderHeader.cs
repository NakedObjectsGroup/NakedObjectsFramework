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
    public record PurchaseOrderHeader : IHasModifiedDate {
        #region ID

        [Hidden]
        public int PurchaseOrderID { get; init; }

        #endregion

        #region Revision Number

        [MemberOrder(90)]
        public byte RevisionNumber { get; init; }

        #endregion

        [Hidden]
        public int ShipMethodID { get; init; }

        [MemberOrder(22)]
#pragma warning disable 8618
        public virtual ShipMethod ShipMethod { get; init; }
#pragma warning restore 8618

        [RenderEagerly]
        [TableView(true, "OrderQty", "Product", "UnitPrice", "LineTotal")]
#pragma warning disable 8618
        public virtual ICollection<PurchaseOrderDetail> Details { get; init; }
#pragma warning restore 8618

        public virtual bool Equals(PurchaseOrderHeader? other) => ReferenceEquals(this, other);

        [MemberOrder(99)]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        public override string ToString() => $"PO from {Vendor}, {OrderDate}";

        public override int GetHashCode() => base.GetHashCode();

        #region Vendor

        [Hidden]
        public int VendorID { get; init; }

        [MemberOrder(1)]
#pragma warning disable 8618
        public virtual Vendor Vendor { get; init; }
#pragma warning restore 8618

        #endregion

        #region Status

        [Hidden]
        public byte Status { get; init; }

        [Named("Status")]
        [MemberOrder(1)]
        public virtual POStatus StatusAsEnum => (POStatus)Status;

        #endregion

        #region Dates

        //Title
        [Mask("d")]
        [MemberOrder(11)]
        public DateTime OrderDate { get; init; }

        [Mask("d")]
        [MemberOrder(20)]
        public DateTime? ShipDate { get; init; }

        #endregion

        #region Amounts

        [MemberOrder(31)]
        [Mask("C")]
        public decimal SubTotal { get; init; }

        [MemberOrder(32)]
        [Mask("C")]
        public decimal TaxAmt { get; init; }

        [MemberOrder(33)]
        [Mask("C")]
        public decimal Freight { get; init; }

        [MemberOrder(34)]
        [Mask("C")]
        public decimal TotalDue { get; init; }

        #endregion

        #region Order Placed By (Employee)

        [Hidden]
        public int OrderPlacedByID { get; init; }

        [MemberOrder(12)]
#pragma warning disable 8618
        public virtual Employee OrderPlacedBy { get; init; }
#pragma warning restore 8618

        #endregion
    }

    public enum POStatus {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        Complete = 4
    }
}