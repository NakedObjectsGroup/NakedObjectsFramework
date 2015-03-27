using System;
using System.Collections.Generic;

namespace AdventureWorks2012CodeFirstModel.Models
{
    public partial class EmployeeAddress
    {
        public int EmployeeID { get; set; }
        public int AddressID { get; set; }
        public System.Guid rowguid { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Address Address { get; set; }
    }
}
