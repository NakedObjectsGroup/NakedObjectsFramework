namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Person.Address")]
    public partial class Address
    {
        public Address()
        {
            EmployeeAddresses = new HashSet<EmployeeAddress>();
            CustomerAddresses = new HashSet<CustomerAddress>();
            SalesOrderHeaders = new HashSet<SalesOrderHeader>();
            SalesOrderHeaders1 = new HashSet<SalesOrderHeader>();
            VendorAddresses = new HashSet<VendorAddress>();
        }

        public int AddressID { get; set; }

        [Required]
        [StringLength(60)]
        public string AddressLine1 { get; set; }

        [StringLength(60)]
        public string AddressLine2 { get; set; }

        [Required]
        [StringLength(30)]
        public string City { get; set; }

        public int StateProvinceID { get; set; }

        [Required]
        [StringLength(15)]
        public string PostalCode { get; set; }

        public Guid rowguid { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<EmployeeAddress> EmployeeAddresses { get; set; }

        public virtual StateProvince StateProvince { get; set; }

        public virtual ICollection<CustomerAddress> CustomerAddresses { get; set; }

        public virtual ICollection<SalesOrderHeader> SalesOrderHeaders { get; set; }

        public virtual ICollection<SalesOrderHeader> SalesOrderHeaders1 { get; set; }

        public virtual ICollection<VendorAddress> VendorAddresses { get; set; }
    }
}
