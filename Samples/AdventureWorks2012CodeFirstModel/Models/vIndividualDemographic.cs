using System;
using System.Collections.Generic;

namespace AdventureWorks2012CodeFirstModel.Models
{
    public partial class vIndividualDemographic
    {
        public int CustomerID { get; set; }
        public Nullable<decimal> TotalPurchaseYTD { get; set; }
        public Nullable<System.DateTime> DateFirstPurchase { get; set; }
        public Nullable<System.DateTime> BirthDate { get; set; }
        public string MaritalStatus { get; set; }
        public string YearlyIncome { get; set; }
        public string Gender { get; set; }
        public Nullable<int> TotalChildren { get; set; }
        public Nullable<int> NumberChildrenAtHome { get; set; }
        public string Education { get; set; }
        public string Occupation { get; set; }
        public Nullable<bool> HomeOwnerFlag { get; set; }
        public Nullable<int> NumberCarsOwned { get; set; }
    }
}
