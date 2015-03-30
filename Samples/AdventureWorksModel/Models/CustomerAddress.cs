using System;
using System.Collections.Generic;

namespace AdventureWorksModel.Models
{
    public partial class CustomerAddress
    {
        public int CustomerID { get; set; }
        public int AddressID { get; set; }
        public int AddressTypeID { get; set; }
        public System.Guid rowguid { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual Address Address { get; set; }
        public virtual AddressType AddressType { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
