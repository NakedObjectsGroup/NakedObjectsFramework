using System;
using System.Collections.Generic;

namespace AdventureWorksModel
{
    public partial class Password
    {
        public virtual int BusinessEntityID { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string PasswordSalt { get; set; }
        public virtual Guid rowguid { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
    }
}
