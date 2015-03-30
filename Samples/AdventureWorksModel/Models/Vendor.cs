using System;
using System.Collections.Generic;

namespace AdventureWorksModel.Models
{
    public partial class Vendor
    {
        public Vendor()
        {
            this.ProductVendors = new List<ProductVendor>();
            this.PurchaseOrderHeaders = new List<PurchaseOrderHeader>();
            this.VendorAddresses = new List<VendorAddress>();
            this.VendorContacts = new List<VendorContact>();
        }

        public int VendorID { get; set; }
        public string AccountNumber { get; set; }
        public string Name { get; set; }
        public byte CreditRating { get; set; }
        public bool PreferredVendorStatus { get; set; }
        public bool ActiveFlag { get; set; }
        public string PurchasingWebServiceURL { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual ICollection<ProductVendor> ProductVendors { get; set; }
        public virtual ICollection<PurchaseOrderHeader> PurchaseOrderHeaders { get; set; }
        public virtual ICollection<VendorAddress> VendorAddresses { get; set; }
        public virtual ICollection<VendorContact> VendorContacts { get; set; }
    }
}
