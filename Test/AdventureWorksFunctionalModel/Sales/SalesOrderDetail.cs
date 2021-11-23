using System;
using NakedFunctions;

namespace AW.Types {
    public class SalesOrderDetail {
        public SalesOrderDetail() { }

        public SalesOrderDetail(SalesOrderDetail cloneFrom)
        {
            SalesOrderID = cloneFrom.SalesOrderID;
            SalesOrderDetailID = cloneFrom.SalesOrderDetailID;
            OrderQty = cloneFrom.OrderQty;
            UnitPrice = cloneFrom.UnitPrice;
            UnitPriceDiscount = cloneFrom.UnitPriceDiscount;
            LineTotal = cloneFrom.LineTotal;
            CarrierTrackingNumber = cloneFrom.CarrierTrackingNumber;
            SalesOrderHeader = cloneFrom.SalesOrderHeader;
            SpecialOfferID = cloneFrom.SpecialOfferID;
            ProductID = cloneFrom.ProductID;
            SpecialOfferProduct = cloneFrom.SpecialOfferProduct;
            ModifiedDate = cloneFrom.ModifiedDate;
            rowguid = cloneFrom.rowguid;
        }

        [Hidden]
        public int SalesOrderID { get; init; }

        [Hidden]
        public int SalesOrderDetailID { get; init; }

        [MemberOrder(15)]
        public short OrderQty { get; init; }
        
        [MemberOrder(20),Mask("C")]
        public decimal UnitPrice { get; init; }
   
        [Named("Discount %"),MemberOrder(30),Mask("P")]
        public decimal UnitPriceDiscount { get; init; }
            
        [MemberOrder(40),Mask("C")]
        public decimal LineTotal { get; init; }
        
        [MemberOrder(50)]
        public string? CarrierTrackingNumber { get; init; }

        [Hidden]
        public virtual SalesOrderHeader SalesOrderHeader { get; init; }
        
        [Hidden]
        public int SpecialOfferID { get; init; }

        [Hidden]
        public int ProductID { get; init; }

        [Hidden]
        public virtual SpecialOfferProduct SpecialOfferProduct { get; init; }

        [MemberOrder(11)]
        public virtual Product Product => SpecialOfferProduct.Product;

        [MemberOrder(12)]
        public virtual SpecialOffer SpecialOffer => SpecialOfferProduct.SpecialOffer;

        [MemberOrder(99), Versioned]
        public DateTime ModifiedDate { get; init; }

        [Hidden]
        public Guid rowguid { get; init; }

        public override string ToString() => $"{OrderQty} x {Product}";
    }
}