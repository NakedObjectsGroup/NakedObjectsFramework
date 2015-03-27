using System;
using System.Collections.Generic;

namespace AdventureWorks2012CodeFirstModel.Models
{
    public partial class Customer
    {
        public Customer()
        {
            this.CustomerAddresses = new List<CustomerAddress>();
            this.SalesOrderHeaders = new List<SalesOrderHeader>();
        }

        public int CustomerID { get; set; }
        public Nullable<int> TerritoryID { get; set; }
        public string AccountNumber { get; set; }
        public string CustomerType { get; set; }
        public System.Guid rowguid { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual SalesTerritory SalesTerritory { get; set; }
        public virtual ICollection<CustomerAddress> CustomerAddresses { get; set; }
        public virtual Individual Individual { get; set; }
        public virtual ICollection<SalesOrderHeader> SalesOrderHeaders { get; set; }
        public virtual Store Store { get; set; }
    }
}
