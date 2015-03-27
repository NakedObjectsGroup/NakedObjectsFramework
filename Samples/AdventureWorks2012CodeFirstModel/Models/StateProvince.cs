using System;
using System.Collections.Generic;

namespace AdventureWorks2012CodeFirstModel.Models
{
    public partial class StateProvince
    {
        public StateProvince()
        {
            this.Addresses = new List<Address>();
            this.SalesTaxRates = new List<SalesTaxRate>();
        }

        public int StateProvinceID { get; set; }
        public string StateProvinceCode { get; set; }
        public string CountryRegionCode { get; set; }
        public bool IsOnlyStateProvinceFlag { get; set; }
        public string Name { get; set; }
        public int TerritoryID { get; set; }
        public System.Guid rowguid { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual CountryRegion CountryRegion { get; set; }
        public virtual ICollection<SalesTaxRate> SalesTaxRates { get; set; }
        public virtual SalesTerritory SalesTerritory { get; set; }
    }
}
