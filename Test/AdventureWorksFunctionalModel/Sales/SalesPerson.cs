using System;
using System.Collections.Generic;
using NakedFunctions;

namespace AW.Types {
    public class SalesPerson : IBusinessEntity {
        public SalesPerson() { }

        public SalesPerson(SalesPerson cloneFrom)
        {
            BusinessEntityID = cloneFrom.BusinessEntityID;
            EmployeeDetails = cloneFrom.EmployeeDetails;
            SalesTerritoryID = cloneFrom.SalesTerritoryID;
            SalesTerritory  = cloneFrom.SalesTerritory;
            SalesQuota = cloneFrom.SalesQuota;
            Bonus = cloneFrom.Bonus;
            CommissionPct = cloneFrom.CommissionPct;
            SalesYTD = cloneFrom.SalesYTD;
            SalesLastYear = cloneFrom.SalesLastYear;
            QuotaHistory = cloneFrom.QuotaHistory;
            TerritoryHistory = cloneFrom.TerritoryHistory;
            ModifiedDate = cloneFrom.ModifiedDate;
            rowguid = cloneFrom.rowguid;
        }

        [Hidden]
        public int BusinessEntityID { get; init; }

        [MemberOrder(10)]
        public virtual Employee EmployeeDetails { get; init; }

        [MemberOrder(11)]
        public virtual Person PersonDetails => EmployeeDetails.PersonDetails;

        [Hidden]
        public int? SalesTerritoryID { get; init; }

        [MemberOrder(20)]
        public virtual SalesTerritory? SalesTerritory { get; init; }

        [MemberOrder(30),Mask("C")]
        public decimal? SalesQuota { get; init; }

        [MemberOrder(40),Mask("C")]
        public decimal Bonus { get; init; }

        [MemberOrder(50)]
        [Mask("P")]
        public decimal CommissionPct { get; init; }

        [MemberOrder(60),Mask("C")]
        public decimal SalesYTD { get; init; }

        [MemberOrder(70),Mask("C")]
        public decimal SalesLastYear { get; init; }

        [TableView(false, "QuotaDate", "SalesQuota")] //Column name deliberately duplicated to test that this is ignored
        public virtual ICollection<SalesPersonQuotaHistory> QuotaHistory { get; init; } = new List<SalesPersonQuotaHistory>();

        [TableView(false, "StartDate", "EndDate", "SalesTerritory")]
        public virtual ICollection<SalesTerritoryHistory> TerritoryHistory { get; init; } = new List<SalesTerritoryHistory>();

        [MemberOrder(99), Versioned]
        public DateTime ModifiedDate { get; init; }

        [Hidden]
        public Guid rowguid { get; init; }
        
        public override string ToString() => $"{EmployeeDetails}";
    }
}