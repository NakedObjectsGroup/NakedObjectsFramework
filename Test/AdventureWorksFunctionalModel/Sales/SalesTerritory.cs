// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedFunctions;

namespace AW.Types {
    [Bounded]
    public record SalesTerritory {
        [Hidden]
        public int TerritoryID { get; init; }

        [MemberOrder(10)]
        public string Name { get; init; } = "";

        [MemberOrder(20)]
        public string CountryRegionCode { get; init; } = "";

        [MemberOrder(30)]
        public string Group { get; init; } = "";

        [MemberOrder(40)] [Mask("C")]
        public decimal SalesYTD { get; init; }

        [MemberOrder(41)] [Mask("C")]
        public decimal SalesLastYear { get; init; }

        [MemberOrder(42)] [Mask("C")]
        public decimal CostYTD { get; init; }

        [MemberOrder(43)] [Mask("C")]
        public decimal CostLastYear { get; init; }

        [Hidden]
        public Guid rowguid { get; init; }

        [MemberOrder(99)]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        [Named("States/Provinces covered")] [TableView(true)] //Table View == List View
        public virtual ICollection<StateProvince> StateProvince { get; init; } = new List<StateProvince>();

        public virtual bool Equals(SalesTerritory? other) => ReferenceEquals(this, other);

        public override string ToString() => Name;

        public override int GetHashCode() => base.GetHashCode();
    }
}