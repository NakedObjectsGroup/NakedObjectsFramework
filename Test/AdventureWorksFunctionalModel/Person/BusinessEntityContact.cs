using System;
using NakedFunctions;

namespace AW.Types {
    [Named("Contact")]
    public record BusinessEntityContact : IHasRowGuid, IHasModifiedDate {
        [Hidden]
        public virtual int BusinessEntityID { get; init; }

        [Hidden]
        public virtual BusinessEntity BusinessEntity { get; init; }

        [Hidden]
        public virtual int PersonID { get; init; }

        [MemberOrder(1)]
        public virtual Person Person { get; init; }

        [Hidden]
        public virtual int ContactTypeID { get; init; }

        [MemberOrder(2)]
        public virtual ContactType ContactType { get; init; }

        public virtual bool Equals(BusinessEntityContact other) => ReferenceEquals(this, other);

        [MemberOrder(99)]
        [Versioned]
        public virtual DateTime ModifiedDate { get; init; }

        [Hidden]
        public virtual Guid rowguid { get; init; }

        public override string ToString() => $"{Person}";

        public override int GetHashCode() => base.GetHashCode();
    }
}