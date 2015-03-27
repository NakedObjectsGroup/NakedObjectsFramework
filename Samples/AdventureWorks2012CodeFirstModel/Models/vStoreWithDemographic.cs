using System;
using System.Collections.Generic;

namespace AdventureWorks2012CodeFirstModel.Models
{
    public partial class vStoreWithDemographic
    {
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public string ContactType { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string Phone { get; set; }
        public string EmailAddress { get; set; }
        public int EmailPromotion { get; set; }
        public string AddressType { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string StateProvinceName { get; set; }
        public string PostalCode { get; set; }
        public string CountryRegionName { get; set; }
        public Nullable<decimal> AnnualSales { get; set; }
        public Nullable<decimal> AnnualRevenue { get; set; }
        public string BankName { get; set; }
        public string BusinessType { get; set; }
        public Nullable<int> YearOpened { get; set; }
        public string Specialty { get; set; }
        public Nullable<int> SquareFeet { get; set; }
        public string Brands { get; set; }
        public string Internet { get; set; }
        public Nullable<int> NumberEmployees { get; set; }
    }
}
