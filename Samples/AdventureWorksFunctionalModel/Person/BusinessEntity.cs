using NakedObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedFunctions;

namespace AdventureWorksModel {

    public class BusinessEntity : IBusinessEntity {
        public BusinessEntity(
            int businessEntityID,
            ICollection<BusinessEntityAddress> addresses,
            ICollection<BusinessEntityContact> contacts,
            Guid businessEntityRowguid,
            DateTime businessEntityModifiedDate
            )
        {
            BusinessEntityID = businessEntityID;
            Addresses = addresses;
            Contacts = contacts;
            BusinessEntityRowguid = businessEntityRowguid;
            BusinessEntityModifiedDate = businessEntityModifiedDate;
        }

        public BusinessEntity() {}

        [NakedObjectsIgnore]
        public virtual int BusinessEntityID { get; set; }

        [NakedObjectsIgnore]
        public virtual Guid BusinessEntityRowguid { get; set; }

        [Hidden(WhenTo.Always)]
        [ConcurrencyCheck]
        public virtual DateTime BusinessEntityModifiedDate { get; set; }

        [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(false,
            nameof(BusinessEntityAddress.AddressType),
            nameof(BusinessEntityAddress.Address))] 
        public virtual ICollection<BusinessEntityAddress> Addresses { get; set; }

        [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(false, "ContactType", "Person")] 
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
            return be.With(x => x.BusinessEntityModifiedDate, now);
        }
        #endregion
    }
}
