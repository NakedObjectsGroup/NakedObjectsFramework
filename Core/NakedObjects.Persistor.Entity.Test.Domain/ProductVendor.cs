// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects;

namespace AdventureWorksModel {
    public partial class ProductVendor {
        #region Primitive Properties

        #region ProductID (Int32)

        [MemberOrder(100)]
        public virtual int ProductID { get; set; }

        #endregion

        #region VendorID (Int32)

        [MemberOrder(110)]
        public virtual int VendorID { get; set; }

        #endregion

        #region AverageLeadTime (Int32)

        [MemberOrder(120)]
        public virtual int AverageLeadTime { get; set; }

        #endregion

        #region StandardPrice (Decimal)

        [MemberOrder(130)]
        public virtual decimal StandardPrice { get; set; }

        #endregion

        #region LastReceiptCost (Decimal)

        [MemberOrder(140), Optionally]
        public virtual Nullable<decimal> LastReceiptCost { get; set; }

        #endregion

        #region LastReceiptDate (DateTime)

        [MemberOrder(150), Optionally, Mask("d")]
        public virtual Nullable<DateTime> LastReceiptDate { get; set; }

        #endregion

        #region MinOrderQty (Int32)

        [MemberOrder(160)]
        public virtual int MinOrderQty { get; set; }

        #endregion

        #region MaxOrderQty (Int32)

        [MemberOrder(170)]
        public virtual int MaxOrderQty { get; set; }

        #endregion

        #region OnOrderQty (Int32)

        [MemberOrder(180), Optionally]
        public virtual Nullable<int> OnOrderQty { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(190), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region Product (Product)

        [MemberOrder(200)]
        public virtual Product Product { get; set; }

        #endregion

        #region UnitMeasure (UnitMeasure)

        [MemberOrder(210)]
        public virtual UnitMeasure UnitMeasure { get; set; }

        #endregion

        #region Vendor (Vendor)

        [MemberOrder(220)]
        public virtual Vendor Vendor { get; set; }

        #endregion

        #endregion
    }
}