using System;
using System.Collections.Generic;

namespace AdventureWorksModel.Models
{
    public partial class SalesPerson
    {
        public SalesPerson()
        {
            this.SalesOrderHeaders = new List<SalesOrderHeader>();
            this.SalesPersonQuotaHistories = new List<SalesPersonQuotaHistory>();
            this.SalesTerritoryHistories = new List<SalesTerritoryHistory>();
            this.Stores = new List<Store>();
        }

        public int SalesPersonID { get; set; }
        public Nullable<int> TerritoryID { get; set; }
        public Nullable<decimal> SalesQuota { get; set; }
        public decimal Bonus { get; set; }
        public decimal CommissionPct { get; set; }
        public decimal SalesYTD { get; set; }
        public decimal SalesLastYear { get; set; }
        public System.Guid rowguid { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual ICollection<SalesOrderHeader> SalesOrderHeaders { get; set; }
        public virtual SalesTerritory SalesTerritory { get; set; }
        public virtual ICollection<SalesPersonQuotaHistory> SalesPersonQuotaHistories { get; set; }
        public virtual ICollection<SalesTerritoryHistory> SalesTerritoryHistories { get; set; }
        public virtual ICollection<Store> Stores { get; set; }
    }
}
