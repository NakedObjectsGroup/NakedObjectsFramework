using NakedFunctions;
using System;

namespace AdventureWorksModel {
    [Bounded]
    public record PhoneNumberType : IHasModifiedDate {
        public PhoneNumberType(int phoneNumberTypeID, string name, DateTime modifiedDate)
        {
            PhoneNumberTypeID = phoneNumberTypeID;
            Name = name;
            ModifiedDate = modifiedDate;
        }

        public PhoneNumberType() { }

        [Hidden]
        public virtual int PhoneNumberTypeID { get; set; }

        [Hidden]
        public virtual string Name { get; set; }

        [Hidden]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }
    }
    public static class PhoneNumberTypeFunctions
    {
        public static PhoneNumberType Updating(PhoneNumberType pnt, [Injected] DateTime now)
        {
            return LifeCycleFunctions.UpdateModified(pnt, now);

        }

        public static string Title(this PhoneNumberType pnt)
        {
            return pnt.CreateTitle(pnt.Name);
        }
    }
}
