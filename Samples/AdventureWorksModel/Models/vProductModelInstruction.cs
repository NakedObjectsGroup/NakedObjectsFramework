using System;
using System.Collections.Generic;

namespace AdventureWorksModel.Models
{
    public partial class vProductModelInstruction
    {
        public int ProductModelID { get; set; }
        public string Name { get; set; }
        public string Instructions { get; set; }
        public Nullable<int> LocationID { get; set; }
        public Nullable<decimal> SetupHours { get; set; }
        public Nullable<decimal> MachineHours { get; set; }
        public Nullable<decimal> LaborHours { get; set; }
        public Nullable<int> LotSize { get; set; }
        public string Step { get; set; }
        public System.Guid rowguid { get; set; }
        public System.DateTime ModifiedDate { get; set; }
    }
}
