using System;
using System.Collections.Generic;

namespace AdventureWorksModel.Models
{
    public partial class ScrapReason
    {
        public ScrapReason()
        {
            this.WorkOrders = new List<WorkOrder>();
        }

        public short ScrapReasonID { get; set; }
        public string Name { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual ICollection<WorkOrder> WorkOrders { get; set; }
    }
}
