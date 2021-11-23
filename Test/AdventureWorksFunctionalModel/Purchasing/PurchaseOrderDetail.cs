






using System;
using NakedFunctions;

namespace AW.Types {
    public class PurchaseOrderDetail {
        public PurchaseOrderDetail() { }

        public PurchaseOrderDetail(PurchaseOrderDetail cloneFrom)
        {
            PurchaseOrderID = cloneFrom.PurchaseOrderID;
            PurchaseOrderDetailID  = cloneFrom.PurchaseOrderDetailID;
            DueDate = cloneFrom.DueDate;
            OrderQty = cloneFrom.OrderQty;
            UnitPrice = cloneFrom.UnitPrice;
            LineTotal = cloneFrom.LineTotal;
            ReceivedQty = cloneFrom.ReceivedQty;
            RejectedQty = cloneFrom.RejectedQty;
            StockedQty = cloneFrom.StockedQty;
            ModifiedDate = cloneFrom.ModifiedDate;
            ProductID = cloneFrom.ProductID;
            Product = cloneFrom.Product;
            PurchaseOrderHeader = cloneFrom.PurchaseOrderHeader;
        }

        [Hidden]
        public int PurchaseOrderID { get; init; }

        [Hidden]
        public int PurchaseOrderDetailID { get; init; }

        [MemberOrder(26),Mask("d")]
        public DateTime DueDate { get; init; }

        [MemberOrder(20)]
        public short OrderQty { get; init; }

        [MemberOrder(22),Mask("C")]
        public decimal UnitPrice { get; init; }

        [MemberOrder(24),Mask("C")]
        public decimal LineTotal { get; init; }

        [MemberOrder(30), Mask("#")]
        public decimal ReceivedQty { get; init; }

        [MemberOrder(32), Mask("#")]
        public decimal RejectedQty { get; init; }

        [MemberOrder(34), Mask("#")]
        public decimal StockedQty { get; init; }

        [MemberOrder(99), Versioned]
        public DateTime ModifiedDate { get; init; }

        [Hidden]
        public int ProductID { get; init; }

        [MemberOrder(10)]
        public virtual Product Product { get; init; }

        [Hidden]
        public virtual PurchaseOrderHeader PurchaseOrderHeader { get; init; }
    
        public override string ToString() => $"{OrderQty} x {Product}";
    }
}