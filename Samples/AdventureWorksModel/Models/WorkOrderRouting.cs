using System;
using System.Collections.Generic;

namespace AdventureWorksModel.Models
{
    public partial class WorkOrderRouting
    {
        public int WorkOrderID { get; set; }
        public int ProductID { get; set; }
        public short OperationSequence { get; set; }
        public short LocationID { get; set; }
        public System.DateTime ScheduledStartDate { get; set; }
        public System.DateTime ScheduledEndDate { get; set; }
        public Nullable<System.DateTime> ActualStartDate { get; set; }
        public Nullable<System.DateTime> ActualEndDate { get; set; }
        public Nullable<decimal> ActualResourceHrs { get; set; }
        public decimal PlannedCost { get; set; }
        public Nullable<decimal> ActualCost { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual Location Location { get; set; }
        public virtual WorkOrder WorkOrder { get; set; }
    }
}
