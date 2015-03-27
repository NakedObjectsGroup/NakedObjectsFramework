using System;
using System.Collections.Generic;

namespace AdventureWorks2012CodeFirstModel.Models
{
    public partial class AddressType
    {
        public AddressType()
        {
            this.CustomerAddresses = new List<CustomerAddress>();
            this.VendorAddresses = new List<VendorAddress>();
        }

        public int AddressTypeID { get; set; }
        public string Name { get; set; }
        public System.Guid rowguid { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual ICollection<CustomerAddress> CustomerAddresses { get; set; }
        public virtual ICollection<VendorAddress> VendorAddresses { get; set; }
    }
}
