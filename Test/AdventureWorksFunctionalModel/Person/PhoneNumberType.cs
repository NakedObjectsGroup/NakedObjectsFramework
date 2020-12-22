using NakedFunctions;
using System;
using System.ComponentModel.DataAnnotations;

namespace AdventureWorksModel {
    [Bounded]
    public record PhoneNumberType : IHasModifiedDate {
        [Hidden]
        public virtual int PhoneNumberTypeID { get; init; }

        [Hidden]
        public virtual string Name { get; init; }

        [Hidden, ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; init; }

        public override string ToString() => Name;
    }
}
