// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

using NakedFunctions;
using NakedFunctions;

namespace AdventureWorksModel {
        public record SalesTerritoryHistory {

        #region Injected Services
        
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

        [MemberOrder(1)]
        [Mask("d")]
        public virtual DateTime StartDate { get; set; }

        [MemberOrder(2)]
        [Mask("d")]
        public virtual DateTime? EndDate { get; set; }

        #region SalesPerson
        [Hidden]
        public virtual int BusinessEntityID { get; set; }

        [MemberOrder(3)]
        public virtual SalesPerson SalesPerson { get; set; }
        #endregion

        #region Sales Territory
        [Hidden]
        public virtual int SalesTerritoryID { get; set; }
        [MemberOrder(4)]
        public virtual SalesTerritory SalesTerritory { get; set; }
        #endregion

        #region ModifiedDate and rowguid

        #region ModifiedDate

        [MemberOrder(99)]
        
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #region rowguid

        [Hidden]
        public virtual Guid rowguid { get; set; }

        #endregion

        #endregion
    }

    public static class SalesTerritoryHistoryFunctions
    {

        public static string Title(this SalesTerritoryHistory t)
        {
            return t.CreateTitle($"{SalesPersonFunctions.Title(t.SalesPerson)} {SalesTerritoryFunctions.Title(t.SalesTerritory)}");
        }
    }
}