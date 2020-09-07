using NakedFunctions;
using System;



namespace AdventureWorksModel {
    [Named("Contact")]
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

        [Hidden]
        public virtual int BusinessEntityID { get; set; }

        [Hidden]
        public virtual BusinessEntity BusinessEntity { get; set; }

        [Hidden]
        public virtual int PersonID { get; set; }
        public virtual Person Person { get; set; }

        [Hidden]
        public virtual int ContactTypeID { get; set; }
        public virtual ContactType ContactType { get; set; }

        [Hidden]
        public virtual Guid rowguid { get; set; }

        [MemberOrder(99)]
        
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
            return bec with {ModifiedDate =  now};
        }
    }
}
