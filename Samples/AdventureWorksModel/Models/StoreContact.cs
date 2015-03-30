using System;
using System.Collections.Generic;

namespace AdventureWorksModel.Models
{
    public partial class StoreContact
    {
        public int CustomerID { get; set; }
        public int ContactID { get; set; }
        public int ContactTypeID { get; set; }
        public System.Guid rowguid { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual Contact Contact { get; set; }
        public virtual ContactType ContactType { get; set; }
        public virtual Store Store { get; set; }
    }
}
