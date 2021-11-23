






using System;
using NakedFunctions;

namespace AW.Types {
    public class CurrencyRate {
        [Hidden]
        public int CurrencyRateID { get; init; }

        public DateTime CurrencyRateDate { get; init; }

        public decimal AverageRate { get; init; }

        public decimal EndOfDayRate { get; init; }

        [Hidden]
        public string FromCurrencyCode { get; init; } = "";


        public virtual Currency Currency { get; init; }


        [Hidden]
        public string ToCurrencyCode { get; init; } = "";


        public virtual Currency Currency1 { get; init; }


        [MemberOrder(99)] [Versioned]
        public DateTime ModifiedDate { get; init; }

        
        public override string ToString() => $"{AverageRate}";


    }
}