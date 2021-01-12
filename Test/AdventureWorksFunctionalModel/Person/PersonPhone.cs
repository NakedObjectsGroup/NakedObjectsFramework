using NakedFunctions;
using static AW.Utilities;
using System;

namespace AW.Types
{
    public record PersonPhone : IHasModifiedDate
    {
        [Hidden]
        public virtual int BusinessEntityID { get; init; }

        public virtual string PhoneNumber { get; init; }

        [Hidden]
        public virtual int PhoneNumberTypeID { get; init; }

        public virtual PhoneNumberType PhoneNumberType { get; init; }
        
        [Versioned]
		public virtual DateTime ModifiedDate { get; init; }

        public override string ToString() => $"{PhoneNumberType}:{PhoneNumber}";

		public override int GetHashCode() => HashCode(this, BusinessEntityID, PhoneNumber.GetHashCode(), PhoneNumberTypeID);
    }
}
