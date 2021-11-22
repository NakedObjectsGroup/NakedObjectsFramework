using System;
using NakedFunctions;

namespace AW.Types {
    public record EmailAddress : IHasRowGuid, IHasModifiedDate {
        [Hidden]
        public int BusinessEntityID { get; init; }

        [Hidden]
        public int EmailAddressID { get; init; }

        [Named("Email Address")]
        public string? EmailAddress1 { get; init; }

        public virtual bool Equals(EmailAddress? other) => ReferenceEquals(this, other);

        [Hidden]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        [Hidden]
        public Guid rowguid { get; init; }

        public override string? ToString() => EmailAddress1;

        public override int GetHashCode() => base.GetHashCode();
    }
}