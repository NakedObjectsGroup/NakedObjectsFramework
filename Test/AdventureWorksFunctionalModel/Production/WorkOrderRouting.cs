






using System;
using NakedFunctions;

namespace AW.Types {
    public class WorkOrderRouting {
        public WorkOrderRouting() { }

        public WorkOrderRouting(WorkOrderRouting cloneFrom)
        {
            WorkOrderID = cloneFrom.WorkOrderID;
            ProductID = cloneFrom.ProductID;
            OperationSequence = cloneFrom.OperationSequence;
            ScheduledStartDate = cloneFrom.ScheduledStartDate;
            ScheduledEndDate = cloneFrom.ScheduledEndDate;
            ActualStartDate = cloneFrom.ActualStartDate;
            ActualEndDate = cloneFrom.ActualEndDate;
            ActualResourceHrs = cloneFrom.ActualResourceHrs;
            PlannedCost = cloneFrom.PlannedCost;
            ActualCost =  cloneFrom.ActualCost;
            WorkOrder = cloneFrom.WorkOrder;
            LocationID = cloneFrom.LocationID;
            Location = cloneFrom.Location;
            ModifiedDate = cloneFrom.ModifiedDate;
        }

        [Hidden]
        public int WorkOrderID { get; init; }

        [Hidden]
        public int ProductID { get; init; }

        [MemberOrder(1)]
        public short OperationSequence { get; init; }

        [MemberOrder(20)]
        public DateTime? ScheduledStartDate { get; init; }

        [MemberOrder(22)]
        public DateTime? ScheduledEndDate { get; init; }

        [MemberOrder(21)]
        [Mask("d")]
        public DateTime? ActualStartDate { get; init; }

        [MemberOrder(23)]
        [Mask("d")]
        public DateTime? ActualEndDate { get; init; }

        [MemberOrder(31)]
        public decimal? ActualResourceHrs { get; init; }

        [MemberOrder(40), Mask("C")]
        public decimal PlannedCost { get; init; }

        [MemberOrder(41),Mask("C")]
        public decimal? ActualCost { get; init; }

        [Hidden]
        public virtual WorkOrder WorkOrder { get; init; }
      
        [Hidden]
        public short LocationID { get; init; }

        [MemberOrder(10)]
        public virtual Location Location { get; init; }

        [MemberOrder(99), Versioned]
        public DateTime ModifiedDate { get; init; }

        public override string ToString() => $"{Location}";
    }
}