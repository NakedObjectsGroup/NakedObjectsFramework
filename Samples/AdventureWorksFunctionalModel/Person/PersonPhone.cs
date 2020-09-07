using NakedFunctions;
using System;

namespace AdventureWorksModel
{
    public record PersonPhone : IHasModifiedDate {

        public PersonPhone(
            int businessEntityID, 
            Person person, 
            PhoneNumberType phoneNumberType, 
            int phoneNumberTypeID, 
            string phoneNumber,
            DateTime modifiedDate)
        {
            BusinessEntityID = businessEntityID;
            Person = person;
            PhoneNumberType = phoneNumberType;
            PhoneNumberTypeID = phoneNumberTypeID;
            PhoneNumber = phoneNumber;
            ModifiedDate = modifiedDate;
        }

        public PersonPhone() { }

        [Hidden]
        public virtual int BusinessEntityID { get; set; }

        public virtual string PhoneNumber { get; set; }

        //[Hidden]
        //public virtual int PersonID { get; set; }

        [Hidden]
        public virtual Person Person { get; set; }

        [Hidden]
        public virtual int PhoneNumberTypeID { get; set; }

        public virtual PhoneNumberType PhoneNumberType { get; set; }

        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }
    }

    public static class PersonPhoneFunctions
    {

        public static PersonPhone Updating(PersonPhone pp, [Injected] DateTime now)
        {
            return LifeCycleFunctions.UpdateModified(pp, now);
        }

        public static string Title(this PersonPhone pp)
        {
           return pp.CreateTitle($"{pp.PhoneNumberType}:{pp.PhoneNumber}");
        }
    }
}
