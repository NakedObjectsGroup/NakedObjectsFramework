using System;
using System.Collections.Generic;

namespace AdventureWorks2012CodeFirstModel.Models
{
    public partial class VendorAddress
    {
        public int VendorID { get; set; }
        public int AddressID { get; set; }
        public int AddressTypeID { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual Address Address { get; set; }
        public virtual AddressType AddressType { get; set; }
        public virtual Vendor Vendor { get; set; }
    }
}
