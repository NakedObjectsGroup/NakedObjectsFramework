using NakedFunctions;
using System;


namespace AW.Types {
    [Named("Contact")]
    public record BusinessEntityContact: IHasRowGuid, IHasModifiedDate {

        [Hidden]
        public virtual int BusinessEntityID { get; init; }

        [Hidden]
        public virtual BusinessEntity BusinessEntity { get; init; }

        [Hidden]
        public virtual int PersonID { get; init; }

        [MemberOrder(1)]
        public virtual Person Person { get; init; }

        [Hidden]
        public virtual int ContactTypeID { get; init; }

        [MemberOrder(2)]
        public virtual ContactType ContactType { get; init; }

        [Hidden]
        public virtual Guid rowguid { get; init; }

        [MemberOrder(99)]
        [Versioned]
		public virtual DateTime ModifiedDate { get; init; }

        public override string ToString() => $"{Person}";
    }
}
