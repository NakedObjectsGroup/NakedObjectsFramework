// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("dog.png")]
    public class SalesPerson : AWDomainObject {

        #region Properties

        private ICollection<SalesTerritoryHistory> _territoryHistory = new List<SalesTerritoryHistory>();

        #region ID

        [Hidden]
        public virtual int SalesPersonID { get; set; }

        #endregion

        #region Employee

        [Title]
        [Disabled]
        [MemberOrder(10)]
        [DisplayName("Employee Details")]
        public virtual Employee Employee { get; set; }

        #endregion

        #region SalesTerritory

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
        public override DateTime ModifiedDate { get; set; }

        #endregion

        #region rowguid

        [Hidden]
        public override Guid rowguid { get; set; }

        #endregion

        #endregion

        #region Quota History

        private ICollection<SalesPersonQuotaHistory> _quotaHistory = new List<SalesPersonQuotaHistory>();

        [TableView(false, "QuotaDate", "SalesQuota")] 
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

        #region RecalulateSalesYTD (Action)

        [MemberOrder(1)]
        public void RecalulateSalesYTD() {
            var startOfYear = new DateTime(DateTime.Now.Year, 1, 1);
            IQueryable<SalesOrderHeader> query = from obj in Container.Instances<SalesOrderHeader>()
                                                 where obj.SalesPerson.SalesPersonID == SalesPersonID &&
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
    }
}