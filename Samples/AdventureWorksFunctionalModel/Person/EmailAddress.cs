using NakedObjects;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NakedFunctions;

namespace AdventureWorksModel {

    public  class EmailAddress : IHasRowGuid, IHasModifiedDate {

        public EmailAddress(
            int businessEntityID,
            int emailAddressID,
            string emailAddress1,
            int personId,
            Person person,
            Guid rowguid,
            DateTime modifiedDate
            )
        {
            BusinessEntityID = businessEntityID;
            EmailAddressID = emailAddressID;
            EmailAddress1 = emailAddress1;
            //PersonId = personId;
            Person = person;
            this.rowguid = rowguid;
            ModifiedDate = modifiedDate;
        }

        public EmailAddress() { }

        [NakedObjectsIgnore]
        public virtual int BusinessEntityID { get; set; }
        [NakedObjectsIgnore]
        public virtual int EmailAddressID { get; set; }

        [DisplayName("Email Address")]
        [RegEx(Validation = @"^[\-\w\.]+@[\-\w\.]+\.[A-Za-z]+$", Message = "Not a valid email address")]
        public virtual string EmailAddress1 { get; set; }

        //[NakedObjectsIgnore]
        //public virtual int PersonId { get; set; }

        [NakedObjectsIgnore]
        public virtual Person Person { get; set; }

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; set; }

        [NakedObjectsIgnore]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }
    }

    public static class EmailAddressFunctions
    {
        public static string Title(this EmailAddress ema)
        {
            return ema.CreateTitle(ema.EmailAddress1);
        }

        public static EmailAddress Updating(EmailAddress ea, [Injected] DateTime now)
        {
            return LifeCycleFunctions.UpdateModified(ea, now);

        }
    }
}
