using NakedFunctions;
using System;
using System.ComponentModel.DataAnnotations;

namespace AdventureWorksModel {
    public  record Password : IHasRowGuid, IHasModifiedDate
    {
        public virtual int BusinessEntityID { get; init; }
        public virtual string PasswordHash { get; init; }
        public virtual string PasswordSalt { get; init; }
        public virtual Guid rowguid { get; init; }
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; init; }
    }

    public static class PasswordFunctions
    {
        public static Password Updating(Password pw, [Injected] DateTime now) => pw with { ModifiedDate = now };
    }
}
