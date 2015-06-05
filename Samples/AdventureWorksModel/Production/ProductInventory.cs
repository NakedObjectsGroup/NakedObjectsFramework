// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("cartons.png")]
    public class ProductInventory : AWDomainObject {
        [Hidden(WhenTo.Always)]
        public virtual int ProductID { get; set; }

        [Hidden(WhenTo.Always)]
        public virtual short LocationID { get; set; }

        [MemberOrder(40)]
        public virtual string Shelf { get; set; }

        [MemberOrder(50)]
        public virtual byte Bin { get; set; }

        [MemberOrder(10)]
        public virtual short Quantity { get; set; }

        [MemberOrder(30)]
        public virtual Location Location { get; set; }

        [MemberOrder(20)]
        public virtual Product Product { get; set; }

        #region Title

        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(Quantity.ToString()).Append(" in", Location).Append(" -", Shelf);
            return t.ToString();
        }

        #endregion

        #region Row Guid and Modified Date

        #region rowguid

        [Hidden(WhenTo.Always)]
        public override Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion

        #endregion
    }
}