using System;
using System.Collections.Generic;
using NakedFunctions;

namespace AW.Types {
    public record BusinessEntity : IBusinessEntity {
        [Hidden]
        public virtual Guid BusinessEntityRowguid { get; init; }

        [Hidden]
        public virtual DateTime BusinessEntityModifiedDate { get; init; }

        [RenderEagerly]
        [TableView(false, nameof(BusinessEntityAddress.AddressType), nameof(BusinessEntityAddress.Address))]
        public virtual ICollection<BusinessEntityAddress> Addresses { get; init; } = new List<BusinessEntityAddress>();

        [RenderEagerly]
        [TableView(false, nameof(BusinessEntityContact.ContactType), nameof(BusinessEntityContact.Person))]
        public virtual ICollection<BusinessEntityContact> Contacts { get; init; } = new List<BusinessEntityContact>();

        [Hidden]
        public virtual int BusinessEntityID { get; init; }

        public virtual bool Equals(BusinessEntity? other) => ReferenceEquals(this, other);

        public override string ToString() => $"BusinessEntity: {BusinessEntityID}";

        public override int GetHashCode() => base.GetHashCode();
    }
}