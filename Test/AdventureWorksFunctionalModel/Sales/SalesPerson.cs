// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedFunctions;

namespace AW.Types
{
    public record SalesPerson : IBusinessEntity
    {
         [Hidden]
        public virtual int BusinessEntityID { get; init; }

        [MemberOrder(10)]
        public virtual Employee EmployeeDetails { get; init; }

        [MemberOrder(11)]
        public virtual Person PersonDetails
        {
            get
            {
                return EmployeeDetails.PersonDetails;
            }
        }

        [Hidden]
        public virtual int? SalesTerritoryID { get; init; }

        [MemberOrder(20)]
        public virtual SalesTerritory SalesTerritory { get; init; }

        [MemberOrder(30)]
        [Mask("C")]
        public virtual decimal? SalesQuota { get; init; }

        [MemberOrder(40)]
        [Mask("C")]
        public virtual decimal Bonus { get; init; }

        [MemberOrder(50)]
        [Mask("P")]
        public virtual decimal CommissionPct { get; init; }

        [MemberOrder(60)]
        [Mask("C")]
        public virtual decimal SalesYTD { get; init; }

        [MemberOrder(70)]
        [Mask("C")]
        public virtual decimal SalesLastYear { get; init; }

        [MemberOrder(99)]
        [Versioned]
		public virtual DateTime ModifiedDate { get; init; }

        [Hidden]
        public virtual Guid rowguid { get; init; }

        [TableView(false, "QuotaDate", "SalesQuota")] //Column name deliberately duplicated to test that this is ignored
        public virtual ICollection<SalesPersonQuotaHistory> QuotaHistory { get; init; }

        [TableView(false, "StartDate", "EndDate", "SalesTerritory")]
        public virtual ICollection<SalesTerritoryHistory> TerritoryHistory { get; init; }
  
        public override string ToString() => $"{EmployeeDetails}";

		public override int GetHashCode() =>base.GetHashCode();

        public virtual bool Equals(SalesPerson other) => ReferenceEquals(this, other);
    }
}