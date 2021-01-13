using NakedFunctions;
using static AW.Utilities;
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
        public virtual Guid rowguid { get; init; }

        [Hidden]
        [Versioned]
		public virtual DateTime ModifiedDate { get; init; }

        public override string ToString() => EmailAddress1;

		public override int GetHashCode() => HashCode(this, BusinessEntityID, EmailAddressID);

        public virtual bool Equals(EmailAddress other) => ReferenceEquals(this, other);
    }
}
