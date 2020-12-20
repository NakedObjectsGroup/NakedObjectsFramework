// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedFunctions;
using static AdventureWorksModel.Helpers;

namespace AdventureWorksModel
{
    public record SalesPerson : IBusinessEntity
    {
         [Hidden]
        public virtual int BusinessEntityID { get; set; }

        [MemberOrder(10)]
        public virtual Employee EmployeeDetails { get; set; }

        [MemberOrder(11)]
        public virtual Person PersonDetails
        {
            get
            {
                return EmployeeDetails.PersonDetails;
            }
        }

        [Hidden]
        public virtual int? SalesTerritoryID { get; set; }

        [MemberOrder(20)]
        public virtual SalesTerritory SalesTerritory { get; set; }

        [MemberOrder(30)]
        [Mask("C")]
        public virtual decimal? SalesQuota { get; set; }

        [MemberOrder(40)]
        [Mask("C")]
        public virtual decimal Bonus { get; set; }

        [MemberOrder(50)]
        [Mask("P")]
        public virtual decimal CommissionPct { get; set; }

        [MemberOrder(60)]
        [Mask("C")]
        public virtual decimal SalesYTD { get; set; }

        [MemberOrder(70)]
        [Mask("C")]
        public virtual decimal SalesLastYear { get; set; }

        [MemberOrder(99), ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        [Hidden]
        public virtual Guid rowguid { get; set; }

        [TableView(false, "QuotaDate", "SalesQuota", "QuotaDate")] //Column name deliberately duplicated to test that this is ignored
        public virtual ICollection<SalesPersonQuotaHistory> QuotaHistory { get; init; } = new List<SalesPersonQuotaHistory>();

        [TableView(false, "StartDate", "EndDate", "SalesTerritory")]
        public virtual ICollection<SalesTerritoryHistory> TerritoryHistory { get; init; } = new List<SalesTerritoryHistory>();
  
        public override string ToString() => $"{EmployeeDetails}";
    }

    public static class SalesPersonFunctions
    {

        #region Life Cycle Methods
        public static SalesPerson Updating(SalesPerson x, IContainer container) => x with { ModifiedDate = container.Now() };

        public static SalesPerson Persisting(SalesPerson x, IContainer container) => x with { rowguid = container.NewGuid(), ModifiedDate = container.Now() };
        #endregion


        [MemberOrder(1)]
        public static (SalesPerson, IContainer) RecalulateSalesYTD(this SalesPerson sp, IContainer container)
        {
            var startOfYear = new DateTime(DateTime.Now.Year, 1, 1);
            decimal newYTD = 0;
            IQueryable<SalesOrderHeader> query = from obj in container.Instances<SalesOrderHeader>()
                                                 where obj.SalesPerson.BusinessEntityID == sp.BusinessEntityID &&
                                                       obj.Status == 5 &&
                                                       obj.OrderDate >= startOfYear
                                                 select obj;
            if (query.Count() > 0)
            {
                newYTD = query.Sum(n => n.SubTotal);
            }
            else
            {
                newYTD = 0;
            }
            return DisplayAndSave(sp with { SalesYTD = newYTD }, container);
        }

        [MemberOrder(2)]
        public static (SalesPerson, IContainer) ChangeSalesQuota(this SalesPerson sp, decimal newQuota, IContainer container)
        {
            var newSP = sp with { SalesQuota = newQuota };
            var history = new SalesPersonQuotaHistory() with { SalesPerson = sp, SalesQuota = newQuota, QuotaDate = container.Now() };
            return (newSP, container.WithPendingSave(newSP, history));
        }

        [MemberOrder(1)]
        public static (SalesPerson, IContainer) ChangeSalesTerritory(this SalesPerson sp, SalesTerritory newTerritory, IContainer container)
        {

            var newHist = new SalesTerritoryHistory() with { SalesPerson = sp, SalesTerritory = newTerritory, StartDate = container.Now() };
            var prev = sp.TerritoryHistory.Where(n => n.EndDate == null).FirstOrDefault();
            var newPrev = prev with { EndDate = container.Now() };
            var newSp = sp with { SalesTerritory = newTerritory };
            return (newSp, container.WithPendingSave(newSp, newPrev, newHist));
        }
    }
}