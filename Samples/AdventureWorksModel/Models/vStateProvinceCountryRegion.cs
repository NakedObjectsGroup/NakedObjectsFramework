using System;
using System.Collections.Generic;

namespace AdventureWorksModel.Models
{
    public partial class vStateProvinceCountryRegion
    {
        public int StateProvinceID { get; set; }
        public string StateProvinceCode { get; set; }
        public bool IsOnlyStateProvinceFlag { get; set; }
        public string StateProvinceName { get; set; }
        public int TerritoryID { get; set; }
        public string CountryRegionCode { get; set; }
        public string CountryRegionName { get; set; }
    }
}
