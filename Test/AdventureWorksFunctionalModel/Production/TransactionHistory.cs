






using System;
using NakedFunctions;

namespace AW.Types {
    public class TransactionHistory {
        public int TransactionID { get; init; }
        public int ReferenceOrderID { get; init; }
        public int ReferenceOrderLineID { get; init; }
        public DateTime TransactionDate { get; init; }
        public string TransactionType { get; init; } = "";
        public int Quantity { get; init; }
        public decimal ActualCost { get; init; }

        [Hidden]
        public int ProductID { get; init; }


        public virtual Product Product { get; init; }


        [MemberOrder(99)]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        
        public override string ToString() => $"TransactionHistory: {TransactionID}";


    }
}