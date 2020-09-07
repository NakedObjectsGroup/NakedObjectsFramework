using NakedFunctions;
using System;
using System.ComponentModel.DataAnnotations;

namespace AdventureWorksModel {
    [Bounded]
    public record PhoneNumberType : IHasModifiedDate {
        [NakedObjectsIgnore]
        public virtual int PhoneNumberTypeID { get; init; }

        [NakedObjectsIgnore]
        public virtual string Name { get; init; }

        [NakedObjectsIgnore, ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; init; }

        public override string ToString() => Name;
    }
    public static class PhoneNumberTypeFunctions
    {
        public static PhoneNumberType Updating(PhoneNumberType pnt, [Injected] DateTime now) => pnt with { ModifiedDate = now };
    }
}
