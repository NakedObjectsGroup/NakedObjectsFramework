using System;
using System.ComponentModel.DataAnnotations;
using NakedFramework;
using NakedObjects;

namespace AdventureWorksLegacyModel.Persons {
    [Bounded]
    public class PhoneNumberType {
        [NakedObjectsIgnore]
        public virtual int PhoneNumberTypeID { get; set; }
        [Title][Hidden(WhenTo.Always)]
        public virtual string Name { get; set; }
        [NakedObjectsIgnore]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }
    }
}
