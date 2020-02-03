namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Production.Location")]
    public partial class Location
    {
        public Location()
        {
            ProductInventories = new HashSet<ProductInventory>();
            WorkOrderRoutings = new HashSet<WorkOrderRouting>();
        }

        public short LocationID { get; set; }

        [Required]
        [StringLength(50)]
        [ConcurrencyCheck]
        public string Name { get; set; }

        [Column(TypeName = "smallmoney")]
        public decimal CostRate { get; set; }

        public decimal Availability { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<ProductInventory> ProductInventories { get; set; }

        public virtual ICollection<WorkOrderRouting> WorkOrderRoutings { get; set; }
    }
}
