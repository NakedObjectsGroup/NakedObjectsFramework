// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects;

namespace AdventureWorksModel {
    // ReSharper disable once PartialTypeWithSinglePart
    // ReSharper disable InconsistentNaming

    public partial class WorkOrder {
        #region Primitive Properties

        #region WorkOrderID (Int32)

        [MemberOrder(100)]
        public virtual int WorkOrderID { get; set; }

        #endregion

        #region OrderQty (Int32)

        [MemberOrder(110)]
        public virtual int OrderQty { get; set; }

        #endregion

        #region StockedQty (Int32)

        [MemberOrder(120)]
        public virtual int StockedQty { get; set; }

        #endregion

        #region ScrappedQty (Int16)

        [MemberOrder(130)]
        public virtual short ScrappedQty { get; set; }

        #endregion

        #region StartDate (DateTime)

        [MemberOrder(140), Mask("d")]
        public virtual DateTime StartDate { get; set; }

        #endregion

        #region EndDate (DateTime)

        [MemberOrder(150), Optionally, Mask("d")]
        public virtual DateTime? EndDate { get; set; }

        #endregion

        #region DueDate (DateTime)

        [MemberOrder(160), Mask("d")]
        public virtual DateTime DueDate { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(170), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region Product (Product)

        [MemberOrder(180)]
        public virtual Product Product { get; set; }

        #endregion

        #region ScrapReason (ScrapReason)

        [MemberOrder(190)]
        public virtual ScrapReason ScrapReason { get; set; }

        #endregion

        #region WorkOrderRoutings (Collection of WorkOrderRouting)

        private ICollection<WorkOrderRouting> _workOrderRoutings = new List<WorkOrderRouting>();

        [MemberOrder(200), Disabled]
        public virtual ICollection<WorkOrderRouting> WorkOrderRoutings {
            get { return _workOrderRoutings; }
            set { _workOrderRoutings = value; }
        }

        #endregion

        #endregion
    }
}