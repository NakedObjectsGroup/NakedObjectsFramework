using NakedFunctions;
using System;
using System.Collections.Generic;


namespace AW.Types {

    public record BusinessEntity : IBusinessEntity
    {

        [Hidden]
        public virtual int BusinessEntityID { get; init; }

        [Hidden]
        public virtual Guid BusinessEntityRowguid { get; init; }

        [Hidden]
        public virtual DateTime BusinessEntityModifiedDate { get; init; }

        [RenderEagerly]
        [TableView(false, nameof(BusinessEntityAddress.AddressType), nameof(BusinessEntityAddress.Address))]
        public virtual ICollection<BusinessEntityAddress> Addresses { get; init; }

        [RenderEagerly]
        [TableView(false, nameof(BusinessEntityContact.ContactType), nameof(BusinessEntityContact.Person))]
        public virtual ICollection<BusinessEntityContact> Contacts { get; init; }

        public override string ToString() => $"BusinessEntity: {BusinessEntityID}";

		public override int GetHashCode() =>base.GetHashCode();

        public virtual bool Equals(BusinessEntity other) => ReferenceEquals(this, other);
    }
}
