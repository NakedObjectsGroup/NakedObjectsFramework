using System;
using System.Collections.Generic;

namespace AdventureWorksModel.Models
{
    public partial class ProductInventory
    {
        public int ProductID { get; set; }
        public short LocationID { get; set; }
        public string Shelf { get; set; }
        public byte Bin { get; set; }
        public short Quantity { get; set; }
        public System.Guid rowguid { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual Location Location { get; set; }
        public virtual Product Product { get; set; }
    }
}
