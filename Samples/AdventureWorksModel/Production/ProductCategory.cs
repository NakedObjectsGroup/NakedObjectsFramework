// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using NakedObjects;

namespace AdventureWorksModel {
    [Bounded]
    [Immutable]
    public class ProductCategory : AWDomainObject {
        private ICollection<ProductSubcategory> _ProductSubcategory = new List<ProductSubcategory>();

        [Hidden(WhenTo.Always)]
        public virtual int ProductCategoryID { get; set; }

        [Title]
        public virtual string Name { get; set; }

        [DisplayName("Subcategories")]
        [TableView(true)] //TableView == ListView
        public virtual ICollection<ProductSubcategory> ProductSubcategory {
            get { return _ProductSubcategory; }
            set { _ProductSubcategory = value; }
        }

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