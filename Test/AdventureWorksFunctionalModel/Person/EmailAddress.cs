using NakedFunctions;
using System;


namespace AW.Types
{
    public record EmailAddress : IHasRowGuid, IHasModifiedDate
    {
        [Hidden]
        public virtual int BusinessEntityID { get; init; }

        [Hidden]
        public virtual int EmailAddressID { get; init; }

        [Named("Email Address")]
        [RegEx(Validation = @"^[\-\w\.]+@[\-\w\.]+\.[A-Za-z]+$", Message = "Not a valid email address")]
        public virtual string EmailAddress1 { get; init; }

        [Hidden]
        public virtual int PersonId { get; init; }

        [Hidden]
        public virtual Person Person { get; init; }

        [Hidden]
        public virtual Guid rowguid { get; init; }

        [Hidden]
        public virtual DateTime ModifiedDate { get; init; }

        public override string ToString() => EmailAddress1;
    }
}
