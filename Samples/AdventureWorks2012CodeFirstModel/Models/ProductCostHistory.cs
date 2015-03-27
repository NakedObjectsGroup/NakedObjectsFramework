using System;
using System.Collections.Generic;

namespace AdventureWorks2012CodeFirstModel.Models
{
    public partial class ProductCostHistory
    {
        public int ProductID { get; set; }
        public System.DateTime StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public decimal StandardCost { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual Product Product { get; set; }
    }
}
