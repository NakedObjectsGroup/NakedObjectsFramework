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
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("dog.png")]
    public class SalesPerson : IBusinessEntity {

        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        #region Life Cycle Methods
        public virtual void Persisting() {
            rowguid = Guid.NewGuid();
            ModifiedDate = DateTime.Now;
        }

        public virtual void Updating() {
            ModifiedDate = DateTime.Now;
        }
        #endregion

        #region RecalulateSalesYTD (Action)

        [MemberOrder(1)]
        public void RecalulateSalesYTD() {
            var startOfYear = new DateTime(DateTime.Now.Year, 1, 1);
            IQueryable<SalesOrderHeader> query = from obj in Container.Instances<SalesOrderHeader>()
                where obj.SalesPerson.BusinessEntityID == BusinessEntityID &&
                      obj.Status == 5 &&
                      obj.OrderDate >= startOfYear
                select obj;
            if (query.Count() > 0) {
                query.Sum(n => n.SubTotal);
            }
            else {
                SalesYTD = 0;
            }
        }

        // Use 'hide', 'dis', 'val', 'actdef', 'actcho' shortcuts to add supporting methods here.

        #endregion

        #region Properties

        private ICollection<SalesTerritoryHistory> _territoryHistory = new List<SalesTerritoryHistory>();

        #region ID

        [NakedObjectsIgnore]
        public virtual int BusinessEntityID { get; set; }

        #endregion

        #region Employee

        [Title]
        [Disabled]
        [MemberOrder(10)]
        public virtual Employee EmployeeDetails { get; set; }

        [MemberOrder(11)]
        public virtual Person PersonDetails {
            get {
                return EmployeeDetails.PersonDetails;
            }
        }

        #endregion

        #region SalesTerritory

        [NakedObjectsIgnore]
        public virtual int? SalesTerritoryID { get; set; }

        [Optionally]
        [MemberOrder(20)]
        public virtual SalesTerritory SalesTerritory { get; set; }

        #endregion

        #region "Sales performance data"

        [Optionally]
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

        #endregion

        #region ModifiedDate and rowguid

        #region ModifiedDate

        [MemberOrder(99), Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #region rowguid

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; set; }

        #endregion

        #endregion

        #region Quota History

        private ICollection<SalesPersonQuotaHistory> _quotaHistory = new List<SalesPersonQuotaHistory>();

        [TableView(false, "QuotaDate", "SalesQuota", "QuotaDate")] //Column name deliberately duplicated to test that this is ignored
        public virtual ICollection<SalesPersonQuotaHistory> QuotaHistory {
            get { return _quotaHistory; }
            set { _quotaHistory = value; }
        }

        #region ChangeSalesQuota (Action)

        [MemberOrder(1)]
        public void ChangeSalesQuota(decimal newQuota) {
            SalesQuota = newQuota;
            var history = Container.NewTransientInstance<SalesPersonQuotaHistory>();
            history.SalesPerson = this;
            history.SalesQuota = newQuota;
            history.QuotaDate = DateTime.Now;
            Container.Persist(ref history);
            QuotaHistory.Add(history);
        }

        #endregion

        #endregion

        #region Territory History

        [TableView(false, "StartDate", "EndDate", "SalesTerritory")]
        public virtual ICollection<SalesTerritoryHistory> TerritoryHistory {
            get { return _territoryHistory; }
            set { _territoryHistory = value; }
        }

        #region ChangeSalesTerritory (Action)

        [MemberOrder(1)]
        public void ChangeSalesTerritory(SalesTerritory newTerritory) {
            SalesTerritory = newTerritory;
            var history = Container.NewTransientInstance<SalesTerritoryHistory>();
            history.SalesPerson = this;
            history.SalesTerritory = newTerritory;
            history.StartDate = DateTime.Now;
            Container.Persist(ref history);
            TerritoryHistory.Add(history);
            CurrentTerritoryHistory().EndDate = DateTime.Now;
        }

        private SalesTerritoryHistory CurrentTerritoryHistory() {
            return TerritoryHistory.Where(n => n.EndDate == null).FirstOrDefault();
        }

        #endregion

        #endregion

        #endregion
    }

    public static class SalesPersonFunctions
    {
        public static string Title(this SalesPerson p)
        {
            return EmployeeFunctions.Title(p.EmployeeDetails);
        }
    }
}