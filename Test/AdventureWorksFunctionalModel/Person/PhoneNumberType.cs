using System;
using NakedFunctions;

namespace AW.Types {
    [Bounded]
    public record PhoneNumberType : IHasModifiedDate {
        [Hidden]
        public int PhoneNumberTypeID { get; init; }

        [Hidden]
        public string? Name { get; init; }

        public virtual bool Equals(PhoneNumberType? other) => ReferenceEquals(this, other);

        [Hidden]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        public override string? ToString() => Name;

        public override int GetHashCode() => base.GetHashCode();
    }
}