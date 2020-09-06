using NakedFunctions;
using System;

namespace AdventureWorksModel {

    public  class EmailAddress : IHasRowGuid, IHasModifiedDate {

        [Hidden]
        public virtual int BusinessEntityID { get; set; }
        [Hidden]
        public virtual int EmailAddressID { get; set; }

        [Named("Email Address")]
        [RegEx(Validation = @"^[\-\w\.]+@[\-\w\.]+\.[A-Za-z]+$", Message = "Not a valid email address")]
        public virtual string EmailAddress1 { get; set; }

        //[Hidden]
        //public virtual int PersonId { get; set; }

        [Hidden]
        public virtual Person Person { get; set; }

        [Hidden]
        public virtual Guid rowguid { get; set; }

        [Hidden]
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
