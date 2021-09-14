using System;
using NakedFunctions;

namespace AW.Types {
    public record EmailAddress : IHasRowGuid, IHasModifiedDate {
        [Hidden]
        public virtual int BusinessEntityID { get; init; }

        [Hidden]
        public virtual int EmailAddressID { get; init; }

        [Named("Email Address")]
        public virtual string EmailAddress1 { get; init; }

        public virtual bool Equals(EmailAddress other) => ReferenceEquals(this, other);

        [Hidden]
        [Versioned]
        public virtual DateTime ModifiedDate { get; init; }

        [Hidden]
        public virtual Guid rowguid { get; init; }

        public override string ToString() => EmailAddress1;

        public override int GetHashCode() => base.GetHashCode();
    }
}