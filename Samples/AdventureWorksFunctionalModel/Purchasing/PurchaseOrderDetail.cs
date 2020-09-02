// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("memo_point.png")]
    public class PurchaseOrderDetail {
        private Product prod;
        private short qty;

        public PurchaseOrderDetail(PurchaseOrderHeader purchaseOrderHeader, Product prod, short qty)
        {
            PurchaseOrderHeader = purchaseOrderHeader;
            this.prod = prod;
            this.qty = qty;
        }
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        #region Life Cycle Methods
        public virtual void Persisting() {
            ModifiedDate = DateTime.Now;
        }

        public virtual void Updating() {
            ModifiedDate = DateTime.Now;
        }
        #endregion
        [NakedObjectsIgnore]
        public virtual int PurchaseOrderID { get; set; }

        [NakedObjectsIgnore]
        public virtual int PurchaseOrderDetailID { get; set; }

        [MemberOrder(26)]
        [Mask("d")]
        public virtual DateTime DueDate { get; set; }

        [MemberOrder(20)]
        public virtual short OrderQty { get; set; }

        [MemberOrder(22)]
        [Mask("C")]
        public virtual decimal UnitPrice { get; set; }

        [MemberOrder(24)]
        [Mask("C")]
        [Disabled]
        public virtual decimal LineTotal { get; set; }

        [Mask("#")]
        [MemberOrder(30)]
        public virtual decimal ReceivedQty { get; set; }

        [Mask("#")]
        [MemberOrder(32)]
        public virtual decimal RejectedQty { get; set; }

        [Mask("#")]
        [MemberOrder(34)]
        [Disabled]
        public virtual decimal StockedQty { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #region Product
        [NakedObjectsIgnore]
        public virtual int ProductID { get; set; }

        [Disabled]
        [MemberOrder(10)]
        public virtual Product Product { get; set; }

        #endregion

        #region Header

        [NakedObjectsIgnore]
        public virtual PurchaseOrderHeader PurchaseOrderHeader { get; set; }

        #endregion

        #region Title

        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(OrderQty.ToString()).Append(" x", Product);
            return t.ToString();
        }

        #endregion

        #region ReceiveGoods (Action)

        [MemberOrder(1)]
        public void ReceiveGoods(int qtyReceived, int qtyRejected, int qtyIntoStock) {
            ReceivedQty = qtyReceived;
            RejectedQty = qtyRejected;
            StockedQty = qtyIntoStock;
        }

        public virtual int Default0ReceiveGoods() {
            return OrderQty;
        }

        public virtual int Default2ReceiveGoods() {
            return OrderQty;
        }

        public virtual string ValidateReceiveGoods(int qtyReceived, int qtyRejected, int qtyIntoStock) {
            var rb = new ReasonBuilder();
            if (qtyRejected + qtyIntoStock != qtyReceived) {
                rb.Append("Qty Into Stock + Qty Rejected must add up to Qty Received");
            }
            return rb.Reason;
        }

        #endregion
    }
}