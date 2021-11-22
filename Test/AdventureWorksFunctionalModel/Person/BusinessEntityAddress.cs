using System;
using NakedFunctions;

namespace AW.Types {
    [Named("Address")]
    public record BusinessEntityAddress : IHasRowGuid, IHasModifiedDate {
        [Hidden]
        public int BusinessEntityID { get; init; }

        [MemberOrder(3)]
#pragma warning disable 8618
        public virtual BusinessEntity BusinessEntity { get; init; }
#pragma warning restore 8618

        [Hidden]
        public int AddressTypeID { get; init; }

        [MemberOrder(1)]
#pragma warning disable 8618
        public virtual AddressType AddressType { get; init; }
#pragma warning restore 8618

        [Hidden]
        public int AddressID { get; init; }

        [MemberOrder(2)]
#pragma warning disable 8618
        public virtual Address Address { get; init; }
#pragma warning restore 8618

        public virtual bool Equals(BusinessEntityAddress? other) => ReferenceEquals(this, other);

        [MemberOrder(99)]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        [Hidden]
        public Guid rowguid { get; init; }

        public override string ToString() => $"{AddressType}: {Address}";

        public override int GetHashCode() => base.GetHashCode();
    }
}