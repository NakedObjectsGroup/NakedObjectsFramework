using NakedFunctions;
using System;
using System.ComponentModel.DataAnnotations;

namespace AdventureWorksModel
{
    [Named("Address")]
    public record BusinessEntityAddress : IHasRowGuid, IHasModifiedDate
    {
        [Hidden]
        public virtual int BusinessEntityID { get; init; }

        [MemberOrder(3)]
        public virtual BusinessEntity BusinessEntity { get; init; }

        [Hidden]
        public virtual int AddressTypeID { get; init; }

        [MemberOrder(1)]
        public virtual AddressType AddressType { get; init; }

        [Hidden]
        public virtual int AddressID { get; init; }

        [MemberOrder(2)]
        public virtual Address Address { get; init; }

        [Hidden]
        public virtual Guid rowguid { get; init; }

        [MemberOrder(99), ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; init; }

        public override string ToString() => $"{AddressType}: {Address}";
    }

}
