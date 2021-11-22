using System;
using NakedFunctions;

namespace AW.Types {
    public record PersonPhone : IHasModifiedDate {
        [Hidden]
        public int BusinessEntityID { get; init; }

        public string? PhoneNumber { get; init; }

        [Hidden]
        public int PhoneNumberTypeID { get; init; }

        public virtual PhoneNumberType? PhoneNumberType { get; init; }

        public virtual bool Equals(PersonPhone? other) => ReferenceEquals(this, other);

        [Versioned]
        public DateTime ModifiedDate { get; init; }

        public override string ToString() => $"{PhoneNumberType}:{PhoneNumber}";

        public override int GetHashCode() => base.GetHashCode();
    }
}