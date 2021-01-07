using NakedFunctions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AW.Types {

    public record BusinessEntity : IBusinessEntity {

        [Hidden]
        public virtual int BusinessEntityID { get; set; }

        [Hidden]
        public virtual Guid BusinessEntityRowguid { get; set; }

        [Hidden, ConcurrencyCheck]
        public virtual DateTime BusinessEntityModifiedDate { get; set; }

        [RenderEagerly]
        [TableView(false, nameof(BusinessEntityAddress.AddressType),nameof(BusinessEntityAddress.Address))] 
        public virtual ICollection<BusinessEntityAddress> Addresses { get; set; }

        [RenderEagerly]
        [TableView(false, nameof(BusinessEntityContact.ContactType),nameof(BusinessEntityContact.Person))] 
        public virtual ICollection<BusinessEntityContact> Contacts { get; set; }
    }
}
