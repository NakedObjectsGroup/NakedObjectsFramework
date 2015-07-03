namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Purchasing.Vendor")]
    public partial class Vendor
    {
        public Vendor()
        {
            ProductVendors = new HashSet<ProductVendor>();
            PurchaseOrderHeaders = new HashSet<PurchaseOrderHeader>();
            VendorAddresses = new HashSet<VendorAddress>();
            VendorContacts = new HashSet<VendorContact>();
        }

        public int VendorID { get; set; }

        [Required]
        [StringLength(15)]
        public string AccountNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public byte CreditRating { get; set; }

        public bool PreferredVendorStatus { get; set; }

        public bool ActiveFlag { get; set; }

        [StringLength(1024)]
        public string PurchasingWebServiceURL { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<ProductVendor> ProductVendors { get; set; }

        public virtual ICollection<PurchaseOrderHeader> PurchaseOrderHeaders { get; set; }

        public virtual ICollection<VendorAddress> VendorAddresses { get; set; }

        public virtual ICollection<VendorContact> VendorContacts { get; set; }
    }
}
