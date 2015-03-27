using System;
using System.Collections.Generic;

namespace AdventureWorks2012CodeFirstModel.Models
{
    public partial class Individual
    {
        public int CustomerID { get; set; }
        public int ContactID { get; set; }
        public string Demographics { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual Contact Contact { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
