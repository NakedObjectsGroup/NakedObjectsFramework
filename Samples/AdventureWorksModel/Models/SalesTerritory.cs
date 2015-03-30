using System;
using System.Collections.Generic;

namespace AdventureWorksModel.Models
{
    public partial class SalesTerritory
    {
        public SalesTerritory()
        {
            this.StateProvinces = new List<StateProvince>();
            this.Customers = new List<Customer>();
            this.SalesOrderHeaders = new List<SalesOrderHeader>();
            this.SalesPersons = new List<SalesPerson>();
            this.SalesTerritoryHistories = new List<SalesTerritoryHistory>();
        }

        public int TerritoryID { get; set; }
        public string Name { get; set; }
        public string CountryRegionCode { get; set; }
        public string Group { get; set; }
        public decimal SalesYTD { get; set; }
        public decimal SalesLastYear { get; set; }
        public decimal CostYTD { get; set; }
        public decimal CostLastYear { get; set; }
        public System.Guid rowguid { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual ICollection<StateProvince> StateProvinces { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<SalesOrderHeader> SalesOrderHeaders { get; set; }
        public virtual ICollection<SalesPerson> SalesPersons { get; set; }
        public virtual ICollection<SalesTerritoryHistory> SalesTerritoryHistories { get; set; }
    }
}
