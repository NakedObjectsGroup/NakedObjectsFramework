namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Production.ScrapReason")]
    public partial class ScrapReason
    {
        public ScrapReason()
        {
            //if (WorkOrders == null) {
            //    WorkOrders = new HashSet<WorkOrder>();
            //}
        }

        public virtual short ScrapReasonID { get; set; }

        [Required]
        [StringLength(50)]
        public virtual string Name { get; set; }

        public virtual DateTime ModifiedDate { get; set; }

        public virtual ICollection<WorkOrder> WorkOrders { get; set; }
    }
}
