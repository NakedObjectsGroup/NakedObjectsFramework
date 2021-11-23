using System;
using NakedFunctions;

namespace AW.Types {
    [Named("Contact")]
    public class BusinessEntityContact : IHasRowGuid, IHasModifiedDate {
        [Hidden]
        public int BusinessEntityID { get; init; }

        [Hidden]
        public virtual BusinessEntity? BusinessEntity { get; init; }

        [Hidden]
        public int PersonID { get; init; }

        [MemberOrder(1)]
        public virtual Person? Person { get; init; }

        [Hidden]
        public int ContactTypeID { get; init; }

        [MemberOrder(2)]
        public virtual ContactType? ContactType { get; init; }

        
        [MemberOrder(99)]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        [Hidden]
        public Guid rowguid { get; init; }

        public override string ToString() => $"{Person}";


    }
}