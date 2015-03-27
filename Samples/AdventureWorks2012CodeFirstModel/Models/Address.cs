using System;
using System.Collections.Generic;

namespace AdventureWorks2012CodeFirstModel.Models
{
    public partial class Address
    {
        public Address()
        {
            this.EmployeeAddresses = new List<EmployeeAddress>();
            this.CustomerAddresses = new List<CustomerAddress>();
            this.SalesOrderHeaders = new List<SalesOrderHeader>();
            this.SalesOrderHeaders1 = new List<SalesOrderHeader>();
            this.VendorAddresses = new List<VendorAddress>();
        }

        public int AddressID { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public int StateProvinceID { get; set; }
        public string PostalCode { get; set; }
        public System.Guid rowguid { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual ICollection<EmployeeAddress> EmployeeAddresses { get; set; }
        public virtual StateProvince StateProvince { get; set; }
        public virtual ICollection<CustomerAddress> CustomerAddresses { get; set; }
        public virtual ICollection<SalesOrderHeader> SalesOrderHeaders { get; set; }
        public virtual ICollection<SalesOrderHeader> SalesOrderHeaders1 { get; set; }
        public virtual ICollection<VendorAddress> VendorAddresses { get; set; }
    }
}
