// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFunctions;

namespace AW.Types {
    public record SalesTerritoryHistory {
        [MemberOrder(1)]
        [Mask("d")]
        public DateTime StartDate { get; init; }

        [MemberOrder(2)]
        [Mask("d")]
        public DateTime? EndDate { get; init; }

        public virtual bool Equals(SalesTerritoryHistory? other) => ReferenceEquals(this, other);

        public override string ToString() => $"{SalesPerson} {SalesTerritory}";

        public override int GetHashCode() => base.GetHashCode();

        #region SalesPerson

        [Hidden]
        public int BusinessEntityID { get; init; }

        [MemberOrder(3)]
#pragma warning disable 8618
        public virtual SalesPerson SalesPerson { get; init; }
#pragma warning restore 8618

        #endregion

        #region Sales Territory

        [Hidden]
        public int SalesTerritoryID { get; init; }

        [MemberOrder(4)]
#pragma warning disable 8618
        public virtual SalesTerritory SalesTerritory { get; init; }
#pragma warning restore 8618

        #endregion

        #region ModifiedDate and rowguid

        #region ModifiedDate

        [MemberOrder(99)]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        #endregion

        #region rowguid

        [Hidden]
        public Guid rowguid { get; init; }

        #endregion

        #endregion
    }
}