






using System;
using NakedFunctions;

namespace AW.Types {
    [Named("Reason")]
    public class SalesOrderHeaderSalesReason {
        [Hidden]
        public int SalesOrderID { get; init; }

        public int SalesReasonID { get; init; }

        public virtual SalesOrderHeader SalesOrderHeader { get; init; }


        public virtual SalesReason SalesReason { get; init; }


        #region ModifiedDate

        [MemberOrder(99)]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        #endregion

        
        public override string ToString() => $"SalesOrderHeaderSalesReason: {SalesOrderID}-{SalesReasonID}";


    }
}