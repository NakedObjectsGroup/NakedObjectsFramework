// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly {
    [Table("Sales.SalesPerson")]
    public class SalesPerson {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SalesPersonID { get; set; }

        public int? TerritoryID { get; set; }

        [Column(TypeName = "money")]
        public decimal? SalesQuota { get; set; }

        [Column(TypeName = "money")]
        public decimal Bonus { get; set; }

        [Column(TypeName = "smallmoney")]
        public decimal CommissionPct { get; set; }

        [Column(TypeName = "money")]
        public decimal SalesYTD { get; set; }

        [Column(TypeName = "money")]
        public decimal SalesLastYear { get; set; }

        public Guid rowguid { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual ICollection<SalesOrderHeader> SalesOrderHeaders { get; set; } = new HashSet<SalesOrderHeader>();

        public virtual ICollection<SalesPersonQuotaHistory> SalesPersonQuotaHistories { get; set; } = new HashSet<SalesPersonQuotaHistory>();

        public virtual ICollection<SalesTerritoryHistory> SalesTerritoryHistories { get; set; } = new HashSet<SalesTerritoryHistory>();

        public virtual ICollection<Store> Stores { get; set; } = new HashSet<Store>();
    }
}