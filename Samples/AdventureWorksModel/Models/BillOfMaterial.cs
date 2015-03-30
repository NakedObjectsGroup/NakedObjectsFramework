using System;
using System.Collections.Generic;

namespace AdventureWorksModel.Models
{
    public partial class BillOfMaterial
    {
        public int BillOfMaterialsID { get; set; }
        public Nullable<int> ProductAssemblyID { get; set; }
        public int ComponentID { get; set; }
        public DateTime StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public string UnitMeasureCode { get; set; }
        public short BOMLevel { get; set; }
        public decimal PerAssemblyQty { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual Product Product { get; set; }
        public virtual Product Product1 { get; set; }
        public virtual UnitMeasure UnitMeasure { get; set; }
    }
}
