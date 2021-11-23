using System;
using NakedFunctions;

namespace AW.Types {
    public class PersonPhone : IHasModifiedDate {
        [Hidden]
        public int BusinessEntityID { get; init; }

        public string? PhoneNumber { get; init; }

        [Hidden]
        public int PhoneNumberTypeID { get; init; }

        public virtual PhoneNumberType? PhoneNumberType { get; init; }

        
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        public override string ToString() => $"{PhoneNumberType}:{PhoneNumber}";


    }
}