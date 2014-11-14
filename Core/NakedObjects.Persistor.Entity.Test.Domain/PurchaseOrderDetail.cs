// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects;

namespace AdventureWorksModel {
    public partial class PurchaseOrderDetail {
        #region Primitive Properties

        #region PurchaseOrderID (Int32)

        [MemberOrder(100)]
        public virtual int PurchaseOrderID { get; set; }

        #endregion

        #region PurchaseOrderDetailID (Int32)

        [MemberOrder(110)]
        public virtual int PurchaseOrderDetailID { get; set; }

        #endregion

        #region DueDate (DateTime)

        [MemberOrder(120), Mask("d")]
        public virtual DateTime DueDate { get; set; }

        #endregion

        #region OrderQty (Int16)

        [MemberOrder(130)]
        public virtual short OrderQty { get; set; }

        #endregion

        #region UnitPrice (Decimal)

        [MemberOrder(140)]
        public virtual decimal UnitPrice { get; set; }

        #endregion

        #region LineTotal (Decimal)

        [MemberOrder(150)]
        public virtual decimal LineTotal { get; set; }

        #endregion

        #region ReceivedQty (Decimal)

        [MemberOrder(160)]
        public virtual decimal ReceivedQty { get; set; }

        #endregion

        #region RejectedQty (Decimal)

        [MemberOrder(170)]
        public virtual decimal RejectedQty { get; set; }

        #endregion

        #region StockedQty (Decimal)

        [MemberOrder(180)]
        public virtual decimal StockedQty { get; set; }

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

        #region PurchaseOrderHeader (PurchaseOrderHeader)

        [MemberOrder(210)]
        public virtual PurchaseOrderHeader PurchaseOrderHeader { get; set; }

        #endregion

        #endregion
    }
}