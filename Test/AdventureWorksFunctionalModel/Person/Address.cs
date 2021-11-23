






using System;
using NakedFunctions;

namespace AW.Types {
    public class Address : IHasRowGuid, IHasModifiedDate {

        public Address() { }

        public Address(Address cloneFrom)
        {
            AddressID = cloneFrom.AddressID;
            AddressLine1 = cloneFrom.AddressLine1;
            AddressLine2 = cloneFrom.AddressLine2;
            City = cloneFrom.City;
            PostalCode = cloneFrom.PostalCode;
            StateProvinceID = cloneFrom.StateProvinceID;
            StateProvince = cloneFrom.StateProvince;
            ModifiedDate = cloneFrom.ModifiedDate;
            rowguid = cloneFrom.rowguid;
        }

        [Hidden]
        public int AddressID { get; init; }

        [MemberOrder(11)]
        public string AddressLine1 { get; init; } = "";

        [MemberOrder(12)]
        public string? AddressLine2 { get; init; }

        [MemberOrder(13)]
        public string City { get; init; } = "";

        [MemberOrder(14)]
        public string PostalCode { get; init; } = "";

        [Hidden]
        public int StateProvinceID { get; init; }

        [MemberOrder(15)]
        public virtual StateProvince StateProvince { get; init; }
        
        [MemberOrder(99)]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        [Hidden]
        public Guid rowguid { get; init; }

        public override string ToString() => $"{AddressLine1}...";


    }
}