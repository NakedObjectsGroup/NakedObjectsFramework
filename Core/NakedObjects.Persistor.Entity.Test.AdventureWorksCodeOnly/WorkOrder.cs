namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Production.WorkOrder")]
    public partial class WorkOrder
    {
        public WorkOrder()
        {
            //WorkOrderRoutings = new HashSet<WorkOrderRouting>();
        }

        public virtual int WorkOrderID { get; set; }

        public virtual int ProductID { get; set; }

        public virtual int OrderQty { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public virtual int StockedQty { get; set; }

        public virtual short ScrappedQty { get; set; }

        public virtual DateTime StartDate { get; set; }

        public virtual DateTime? EndDate { get; set; }

        public virtual DateTime DueDate { get; set; }

        public virtual short? ScrapReasonID { get; set; }

        public virtual DateTime ModifiedDate { get; set; }

        public virtual Product Product { get; set; }

        public virtual ScrapReason ScrapReason { get; set; }

        public virtual ICollection<WorkOrderRouting> WorkOrderRoutings { get; set; }
    }
}
