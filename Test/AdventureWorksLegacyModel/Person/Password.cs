using System;
using System.ComponentModel.DataAnnotations;

namespace AdventureWorksModel {
    public partial class Password
    {
        public virtual int BusinessEntityID { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string PasswordSalt { get; set; }
        public virtual Guid rowguid { get; set; }
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }
    }
}
