






using System;
using NakedFunctions;

namespace AW.Types {
    [Bounded]
    public class Currency {
        [Hidden]
        public string CurrencyCode { get; init; } = "";

        [Hidden]
        public string Name { get; init; } = "";

        [Hidden]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        
        public override string ToString() => $"{CurrencyCode} - {Name}";


    }
}