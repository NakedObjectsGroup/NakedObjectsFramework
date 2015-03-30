// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects;

namespace AdventureWorksModel {
    public class WorkOrderRouting : AWDomainObject {
        [NakedObjectsIgnore]
        public virtual int WorkOrderID { get; set; }

        [NakedObjectsIgnore]
        public virtual int ProductID { get; set; }

        [Disabled]
        [MemberOrder(1)]
        public virtual short OperationSequence { get; set; }

        [MemberOrder(20)]
        [Mask("d")]
        public virtual DateTime? ScheduledStartDate { get; set; }

        [MemberOrder(22)]
        [Mask("d")]
        public virtual DateTime? ScheduledEndDate { get; set; }

        [Optionally]
        [MemberOrder(21)]
        [Mask("d")]
        public virtual DateTime? ActualStartDate { get; set; }

        [Optionally]
        [MemberOrder(23)]
        [Mask("d")]
        public virtual DateTime? ActualEndDate { get; set; }

        [Optionally]
        [MemberOrder(31)]
        public virtual decimal? ActualResourceHrs { get; set; }

        [Mask("C")]
        [MemberOrder(40)]
        public virtual decimal PlannedCost { get; set; }

        [Optionally]
        [MemberOrder(41)]
        [Mask("C")]
        public virtual decimal? ActualCost { get; set; }

        #region Location
        [NakedObjectsIgnore]
        public virtual int LocationID { get; set; }

        [MemberOrder(10)]
        public virtual Location Location { get; set; }
        #endregion

        [NakedObjectsIgnore]
        public virtual WorkOrder WorkOrder { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion

        #region Title

        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(Location);
            return t.ToString();
        }

        #endregion

        public virtual string ValidatePlannedCost(decimal plannedCost) {
            var rb = new ReasonBuilder();
            if (plannedCost <= 0) {
                rb.Append("Planned cost must be > 0");
            }
            return rb.Reason;
        }
    }
}