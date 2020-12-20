using NakedFunctions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdventureWorksModel {

    public record BusinessEntity : IBusinessEntity {

        public BusinessEntity() {}

        [Hidden]
        public virtual int BusinessEntityID { get; set; }

        [Hidden]
        public virtual Guid BusinessEntityRowguid { get; set; }

        [Hidden, ConcurrencyCheck]
        public virtual DateTime BusinessEntityModifiedDate { get; set; }

        [RenderEagerly]
        [TableView(false,
            nameof(BusinessEntityAddress.AddressType),
            nameof(BusinessEntityAddress.Address))] 
        public virtual ICollection<BusinessEntityAddress> Addresses { get; set; }

        [RenderEagerly, TableView(false, "ContactType", "Person")] 
        public virtual ICollection<BusinessEntityContact> Contacts { get; set; }
    }

    public static class BusinessEntityFunctions
    {
        public static bool HideContacts(BusinessEntity be)
        {
            return false;
        }

        //TODO: This needs modification to create persisted address with all fields filled.
        public static Address CreateNewAddress(BusinessEntity be)
        {
            var a = new Address();  //TODO add all fields
            //a.AddressFor = be;
            return a;
        }


        #region Life Cycle Methods
        public static BusinessEntity Updating(BusinessEntity be, [Injected] DateTime now)
        {
            return be with {BusinessEntityModifiedDate =  now};
        }
        #endregion
    }
}
