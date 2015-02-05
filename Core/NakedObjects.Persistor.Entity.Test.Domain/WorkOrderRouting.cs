// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects;

namespace AdventureWorksModel {
    // ReSharper disable once PartialTypeWithSinglePart
    // ReSharper disable InconsistentNaming

    public partial class WorkOrderRouting {
        #region Primitive Properties

        #region WorkOrderID (Int32)

        [MemberOrder(100)]
        public virtual int WorkOrderID { get; set; }

        #endregion

        #region ProductID (Int32)

        [MemberOrder(110)]
        public virtual int ProductID { get; set; }

        #endregion

        #region OperationSequence (Int16)

        [MemberOrder(120)]
        public virtual short OperationSequence { get; set; }

        #endregion

        #region ScheduledStartDate (DateTime)

        [MemberOrder(130), Mask("d")]
        public virtual DateTime ScheduledStartDate { get; set; }

        #endregion

        #region ScheduledEndDate (DateTime)

        [MemberOrder(140), Mask("d")]
        public virtual DateTime ScheduledEndDate { get; set; }

        #endregion

        #region ActualStartDate (DateTime)

        [MemberOrder(150), Optionally, Mask("d")]
        public virtual DateTime? ActualStartDate { get; set; }

        #endregion

        #region ActualEndDate (DateTime)

        [MemberOrder(160), Optionally, Mask("d")]
        public virtual DateTime? ActualEndDate { get; set; }

        #endregion

        #region ActualResourceHrs (Decimal)

        [MemberOrder(170), Optionally]
        public virtual decimal? ActualResourceHrs { get; set; }

        #endregion

        #region PlannedCost (Decimal)

        [MemberOrder(180)]
        public virtual decimal PlannedCost { get; set; }

        #endregion

        #region ActualCost (Decimal)

        [MemberOrder(190), Optionally]
        public virtual decimal? ActualCost { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(200), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region Location (Location)

        [MemberOrder(210)]
        public virtual Location Location { get; set; }

        #endregion

        #region WorkOrder (WorkOrder)

        [MemberOrder(220)]
        public virtual WorkOrder WorkOrder { get; set; }

        #endregion

        #endregion
    }
}

// ReSharper restore InconsistentNaming