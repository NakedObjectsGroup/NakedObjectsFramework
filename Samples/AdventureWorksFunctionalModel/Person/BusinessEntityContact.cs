using NakedObjects;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NakedFunctions;

namespace AdventureWorksModel {
    [DisplayName("Contact")]
    public  class BusinessEntityContact: IHasRowGuid, IHasModifiedDate {

        //TODO: Ensure constructor includes all properties
        public BusinessEntityContact(
            int businessEntityId,
            BusinessEntity businessEntity,
            int personID,
            Person person,
            int contactTypeID,
            ContactType contactType,
            Guid rowguid,
            DateTime ModifiedDate
            )
        {
            BusinessEntityID = businessEntityId;
            BusinessEntity = businessEntity;
            PersonID = personID;
            Person = person;
            ContactTypeID = contactTypeID;
            ContactType = contactType;
            this.rowguid = rowguid;
            this.ModifiedDate = ModifiedDate;
        }
        public BusinessEntityContact() { }

        [NakedObjectsIgnore]
        public virtual int BusinessEntityID { get; set; }

        [NakedObjectsIgnore]
        public virtual BusinessEntity BusinessEntity { get; set; }

        [NakedObjectsIgnore]
        public virtual int PersonID { get; set; }
        public virtual Person Person { get; set; }

        [NakedObjectsIgnore]
        public virtual int ContactTypeID { get; set; }
        public virtual ContactType ContactType { get; set; }

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; set; }

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }
    }

    public static class BusinessEntityContactFunctions
    {
        public static string Title(this BusinessEntityContact bec)
        {
            return bec.CreateTitle($"{ContactTypeFunctions.Title(bec.ContactType)}: {PersonFunctions.Title(bec.Person)}");
        }

        public static BusinessEntityContact Updating(BusinessEntityContact bec, [Injected] DateTime now)
        {
            return bec.With(x => x.ModifiedDate, now);
        }
    }
}
