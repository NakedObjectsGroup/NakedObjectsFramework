using System;
using System.Collections.Generic;

namespace AdventureWorks2012CodeFirstModel.Models
{
    public partial class ProductDocument
    {
        public int ProductID { get; set; }
        public int DocumentID { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual Document Document { get; set; }
        public virtual Product Product { get; set; }
    }
}
