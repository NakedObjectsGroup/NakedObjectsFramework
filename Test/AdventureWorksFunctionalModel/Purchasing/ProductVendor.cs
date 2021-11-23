






using System;
using NakedFunctions;

namespace AW.Types {
    public class ProductVendor {
        [Hidden]
        public int ProductID { get; init; }

        [Hidden]
        public int VendorID { get; init; }

        [MemberOrder(30)]
        public int AverageLeadTime { get; init; }

        [Mask("C")]
        [MemberOrder(40)]
        public decimal StandardPrice { get; init; }

        [Mask("C")]
        [MemberOrder(41)]
        public decimal? LastReceiptCost { get; init; }

        [Mask("d")]
        [MemberOrder(50)]
        public DateTime? LastReceiptDate { get; init; }

        [MemberOrder(60)]
        public int MinOrderQty { get; init; }

        [MemberOrder(61)]
        public int MaxOrderQty { get; init; }

        [MemberOrder(62)]
        public int? OnOrderQty { get; init; }

        [MemberOrder(10)]

        public virtual Product Product { get; init; }


        [Hidden]
        public string UnitMeasureCode { get; init; } = "";

        [MemberOrder(20)]

        public virtual UnitMeasure UnitMeasure { get; init; }


        [Hidden]

        public virtual Vendor Vendor { get; init; }


        [MemberOrder(99)]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        
        public override string ToString() => $"ProductVendor: {ProductID}-{VendorID}";


    }
}