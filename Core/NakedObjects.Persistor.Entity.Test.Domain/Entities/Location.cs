// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel {
    // ReSharper disable once PartialTypeWithSinglePart
    // ReSharper disable InconsistentNaming

    public partial class Location {
        #region Primitive Properties

        #region LocationID (Int16)

        [MemberOrder(100)]
        public virtual short LocationID { get; set; }

        #endregion

        #region Name (String)

        [MemberOrder(110), StringLength(50)]
        public virtual string Name { get; set; }

        #endregion

        #region CostRate (Decimal)

        [MemberOrder(120)]
        public virtual decimal CostRate { get; set; }

        #endregion

        #region Availability (Decimal)

        [MemberOrder(130)]
        public virtual decimal Availability { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(140), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region ProductInventories (Collection of ProductInventory)

        private ICollection<ProductInventory> _productInventories = new List<ProductInventory>();

        [MemberOrder(150), Disabled]
        public virtual ICollection<ProductInventory> ProductInventories {
            get { return _productInventories; }
            set { _productInventories = value; }
        }

        #endregion

        #region WorkOrderRoutings (Collection of WorkOrderRouting)

        private ICollection<WorkOrderRouting> _workOrderRoutings = new List<WorkOrderRouting>();

        [MemberOrder(160), Disabled]
        public virtual ICollection<WorkOrderRouting> WorkOrderRoutings {
            get { return _workOrderRoutings; }
            set { _workOrderRoutings = value; }
        }

        #endregion

        #endregion
    }
}