// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("memo_point.png")]
    public class PurchaseOrderDetail : AWDomainObject {
        #region Title

        public override string ToString() {
            var t = new TitleBuilder();
            t.Append(OrderQty.ToString()).Append(" x", Product);
            return t.ToString();
        }


        #endregion

        [Hidden]
        public virtual int PurchaseOrderID { get; set; }

        [Hidden]
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
        public virtual decimal LineTotal { get; set; }

        [Mask("#")]
        [MemberOrder(30)]
        public virtual decimal ReceivedQty { get; set; }

        [Mask("#")]
        [MemberOrder(32)]
        public virtual decimal RejectedQty { get; set; }

        [Mask("#")]
        [MemberOrder(34)]
        public virtual decimal StockedQty { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion

        #region Product

        [Disabled]
        [MemberOrder(10)]
        public virtual Product Product { get; set; }

        #endregion

        #region Header

        [Hidden]
        public virtual PurchaseOrderHeader PurchaseOrderHeader { get; set; }

        #endregion

        #region ReceiveGoods (Action)

        [MemberOrder(1)]
        public void ReceiveGoods(int qtyReceived, int qtyRejected, int qtyIntoStock) {
            ReceivedQty = qtyReceived;
            RejectedQty = qtyRejected;
            StockedQty = qtyIntoStock;
            //TODO:  Update inventory level.
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