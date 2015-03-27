using System;
using System.Collections.Generic;

namespace AdventureWorks2012CodeFirstModel.Models
{
    public partial class vSalesPerson
    {
        public int SalesPersonID { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string JobTitle { get; set; }
        public string Phone { get; set; }
        public string EmailAddress { get; set; }
        public int EmailPromotion { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string StateProvinceName { get; set; }
        public string PostalCode { get; set; }
        public string CountryRegionName { get; set; }
        public string TerritoryName { get; set; }
        public string TerritoryGroup { get; set; }
        public Nullable<decimal> SalesQuota { get; set; }
        public decimal SalesYTD { get; set; }
        public decimal SalesLastYear { get; set; }
    }
}
