using System;
using System.Collections.Generic;

namespace AdventureWorks2012CodeFirstModel.Models
{
    public partial class ShipMethod
    {
        public ShipMethod()
        {
            this.PurchaseOrderHeaders = new List<PurchaseOrderHeader>();
            this.SalesOrderHeaders = new List<SalesOrderHeader>();
        }

        public int ShipMethodID { get; set; }
        public string Name { get; set; }
        public decimal ShipBase { get; set; }
        public decimal ShipRate { get; set; }
        public System.Guid rowguid { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual ICollection<PurchaseOrderHeader> PurchaseOrderHeaders { get; set; }
        public virtual ICollection<SalesOrderHeader> SalesOrderHeaders { get; set; }
    }
}
