using NakedFunctions;
using System;


namespace AW.Types {
    [Bounded]
    public record PhoneNumberType : IHasModifiedDate {
        [Hidden]
        public virtual int PhoneNumberTypeID { get; init; }

        [Hidden]
        public virtual string Name { get; init; }

        [Hidden]
        [Versioned]
		public virtual DateTime ModifiedDate { get; init; }

        public override string ToString() => Name;
    }
}
