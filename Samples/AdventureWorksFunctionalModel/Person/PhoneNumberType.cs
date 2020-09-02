using NakedObjects;
using System;
using System.ComponentModel.DataAnnotations;
using NakedFunctions;

namespace AdventureWorksModel {
    [Bounded]
    public class PhoneNumberType : IHasModifiedDate {
        public PhoneNumberType(int phoneNumberTypeID, string name, DateTime modifiedDate)
        {
            PhoneNumberTypeID = phoneNumberTypeID;
            Name = name;
            ModifiedDate = modifiedDate;
        }

        public PhoneNumberType() { }

        [NakedObjectsIgnore]
        public virtual int PhoneNumberTypeID { get; set; }

        [Hidden(WhenTo.Always)]
        public virtual string Name { get; set; }

        [NakedObjectsIgnore]
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
