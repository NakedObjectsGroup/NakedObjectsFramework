// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

using NakedFunctions;
using static AW.Utilities;

namespace AW.Types {
        public record SalesTerritoryHistory {

        [MemberOrder(1)]
        [Mask("d")]
        public virtual DateTime StartDate { get; init; }

        [MemberOrder(2)]
        [Mask("d")]
        public virtual DateTime? EndDate { get; init; }

        #region SalesPerson
        [Hidden]
        public virtual int BusinessEntityID { get; init; }

        [MemberOrder(3)]
        public virtual SalesPerson SalesPerson { get; init; }
        #endregion

        #region Sales Territory
        [Hidden]
        public virtual int SalesTerritoryID { get; init; }
        [MemberOrder(4)]
        public virtual SalesTerritory SalesTerritory { get; init; }
        #endregion

        #region ModifiedDate and rowguid

        #region ModifiedDate

        [MemberOrder(99)]
        
        
        [Versioned]
		public virtual DateTime ModifiedDate { get; init; }

        #endregion

        #region rowguid

        [Hidden]
        public virtual Guid rowguid { get; init; }

        #endregion

        #endregion

        public override string ToString()  => $"{SalesPerson} {SalesTerritory}";

		public override int GetHashCode() => HashCode(this, BusinessEntityID, SalesTerritoryID, StartDate.GetHashCode());

        public virtual bool Equals(SalesTerritoryHistory other) => ReferenceEquals(this, other);
    }
}