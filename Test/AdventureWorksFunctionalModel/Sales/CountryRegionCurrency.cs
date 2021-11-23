






using System;
using NakedFunctions;

namespace AW.Types {
    public class CountryRegionCurrency {
        public string CountryRegionCode { get; init; } = "";
        public string CurrencyCode { get; init; } = "";

        public virtual CountryRegion CountryRegion { get; init; }


        public virtual Currency Currency { get; init; }


        [Versioned]
        public DateTime ModifiedDate { get; init; }

        
        public override string ToString() => $"CountryRegionCurrency: {CountryRegion} {Currency}";


    }
}