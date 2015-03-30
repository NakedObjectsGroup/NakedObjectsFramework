using System;
using System.Collections.Generic;

namespace AdventureWorksModel.Models
{
    public partial class VendorContact
    {
        public int VendorID { get; set; }
        public int ContactID { get; set; }
        public int ContactTypeID { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual Contact Contact { get; set; }
        public virtual ContactType ContactType { get; set; }
        public virtual Vendor Vendor { get; set; }
    }
}
