// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedFunctions;

namespace AdventureWorksModel
{
    [Bounded]
    public record SalesTerritory
    {

        [NakedFunctionsIgnore]
        public virtual int TerritoryID { get; set; }

        [MemberOrder(10)]
        public virtual string Name { get; set; }

        [MemberOrder(20)]
        public virtual string CountryRegionCode { get; set; }

        [MemberOrder(30)]
        public virtual string Group { get; set; }

        [MemberOrder(40), Mask("C")]
        public virtual decimal SalesYTD { get; set; }

        [MemberOrder(41), Mask("C")]
        public virtual decimal SalesLastYear { get; set; }

        [MemberOrder(42), Mask("C")]
        public virtual decimal CostYTD { get; set; }

        [MemberOrder(43), Mask("C")]
        public virtual decimal CostLastYear { get; set; }

        [NakedFunctionsIgnore]
        public virtual Guid rowguid { get; set; }

        [MemberOrder(99), ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        [Named("States/Provinces covered"), TableView(true)] //Table View == List View
        public virtual ICollection<StateProvince> StateProvince { get; init; } = new List<StateProvince>();

        public override string ToString() => Name;
    }

    public static class SalesTerritoryFunctions
    {
        #region Life Cycle Methods
        public static Department Updating(this Department x, [Injected] DateTime now) => x with { ModifiedDate = now };

        public static Department Persisting(this Department x, [Injected] DateTime now) => x with { ModifiedDate = now };
        #endregion
    }
}