// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("clipboard.png")]
    public class SalesTerritoryHistory : AWDomainObject {
        [Hidden]
        public virtual int SalesPersonID { get; set; }

        [Hidden]
        public virtual int TerritoryID { get; set; }

        [MemberOrder(1)]
        [Mask("d")]
        public virtual DateTime StartDate { get; set; }

        [MemberOrder(2)]
        [Mask("d")]
        public virtual DateTime? EndDate { get; set; }

        [MemberOrder(3)]
        public virtual SalesPerson SalesPerson { get; set; }

        [MemberOrder(4)]
        public virtual SalesTerritory SalesTerritory { get; set; }

        #region Title

        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(SalesPerson).Append(" -", SalesTerritory);
            return t.ToString();
        }

        #endregion

        #region ModifiedDate and rowguid

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion

        #region rowguid

        [Hidden]
        public override Guid rowguid { get; set; }

        #endregion

        #endregion
    }
}