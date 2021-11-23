namespace AW.Types;

    [Named("Address")]
    public class BusinessEntityAddress : IHasRowGuid, IHasModifiedDate {
        [Hidden]
        public int BusinessEntityID { get; init; }

        [MemberOrder(3)]
        public virtual BusinessEntity BusinessEntity { get; init; }


        [Hidden]
        public int AddressTypeID { get; init; }

        [MemberOrder(1)]
        public virtual AddressType AddressType { get; init; }

        [Hidden]
        public int AddressID { get; init; }

        [MemberOrder(2)]

        public virtual Address Address { get; init; }

        
        [MemberOrder(99),Versioned]
        public DateTime ModifiedDate { get; init; }

        [Hidden]
        public Guid rowguid { get; init; }

        public override string ToString() => $"{AddressType}: {Address}";
    }