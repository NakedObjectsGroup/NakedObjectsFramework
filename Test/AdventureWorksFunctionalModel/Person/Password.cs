using System;
using NakedFunctions;

namespace AW.Types {
    public record Password : IHasRowGuid, IHasModifiedDate {
        //[Hidden]
        public virtual int BusinessEntityID { get; init; }

        // [Hidden]
#pragma warning disable 8618
        public virtual Person Person { get; init; }
#pragma warning restore 8618

        //[Hidden]
        public virtual string PasswordHash { get; init; } = "";

        //[Hidden]
        public virtual string PasswordSalt { get; init; } = "";

        public virtual bool Equals(Password? other) => ReferenceEquals(this, other);

        // [Hidden]
        [Versioned]
        public virtual DateTime ModifiedDate { get; init; }

        //[Hidden]
        public virtual Guid rowguid { get; init; }

        public override string ToString() => "Password";

        public override int GetHashCode() => base.GetHashCode();
    }
}