// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    public class WorkOrderRouting : AWDomainObject {
        #region Title

        public override string ToString() {
            var t = new TitleBuilder();
            t.Append(Location);
            return t.ToString();
        }

        #endregion


        [Hidden]
        public virtual int WorkOrderID { get; set; }

        [Hidden]
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

        [MemberOrder(10)]
        public virtual Location Location { get; set; }

        [Hidden]
        public virtual WorkOrder WorkOrder { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

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