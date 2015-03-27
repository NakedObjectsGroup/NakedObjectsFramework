using System;
using System.Collections.Generic;

namespace AdventureWorks2012CodeFirstModel.Models
{
    public partial class ContactType
    {
        public ContactType()
        {
            this.StoreContacts = new List<StoreContact>();
            this.VendorContacts = new List<VendorContact>();
        }

        public int ContactTypeID { get; set; }
        public string Name { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual ICollection<StoreContact> StoreContacts { get; set; }
        public virtual ICollection<VendorContact> VendorContacts { get; set; }
    }
}
