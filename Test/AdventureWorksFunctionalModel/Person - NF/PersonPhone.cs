using NakedFunctions;
using System;
using System.ComponentModel.DataAnnotations;

namespace AdventureWorksModel
{
    public record PersonPhone : IHasModifiedDate
    {

        [Hidden]
        public virtual int BusinessEntityID { get; init; }

        public virtual string PhoneNumber { get; init; }

        [Hidden]
        public virtual int PersonID { get; init; }

        [Hidden]
        public virtual Person Person { get; init; }

        [Hidden]
        public virtual int PhoneNumberTypeID { get; init; }

        public virtual PhoneNumberType PhoneNumberType { get; init; }

        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; init; }

        public override string ToString() => $"{PhoneNumberType}:{PhoneNumber}";
    }
}
