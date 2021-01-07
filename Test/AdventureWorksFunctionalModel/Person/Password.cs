using NakedFunctions;
using System;
using System.ComponentModel.DataAnnotations;

namespace AW.Types {
    public  record Password : IHasRowGuid, IHasModifiedDate
    {
        [Hidden]
        public virtual int BusinessEntityID { get; init; }

        [Hidden]
        public virtual string PasswordHash { get; init; }

        [Hidden]
        public virtual string PasswordSalt { get; init; }

        [Hidden]
        public virtual Guid rowguid { get; init; }

        [Hidden, ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; init; }

        public override string ToString() => "Password";
    }
}
