using System;
using System.Collections.Generic;

namespace AdventureWorks2012CodeFirstModel.Models
{
    public partial class BillOfMaterial
    {
        public int BillOfMaterialsID { get; set; }
        public Nullable<int> ProductAssemblyID { get; set; }
        public int ComponentID { get; set; }
        public System.DateTime StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string UnitMeasureCode { get; set; }
        public short BOMLevel { get; set; }
        public decimal PerAssemblyQty { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual Product Product { get; set; }
        public virtual Product Product1 { get; set; }
        public virtual UnitMeasure UnitMeasure { get; set; }
    }
}
