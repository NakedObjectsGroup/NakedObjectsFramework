namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Person.AddressType")]
    public partial class AddressType
    {
        public AddressType()
        {
            CustomerAddresses = new HashSet<CustomerAddress>();
            VendorAddresses = new HashSet<VendorAddress>();
        }

        public int AddressTypeID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public Guid rowguid { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<CustomerAddress> CustomerAddresses { get; set; }

        public virtual ICollection<VendorAddress> VendorAddresses { get; set; }
    }
}
