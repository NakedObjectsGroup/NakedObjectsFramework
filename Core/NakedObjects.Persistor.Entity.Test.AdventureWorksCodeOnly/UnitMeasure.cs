namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Production.UnitMeasure")]
    public partial class UnitMeasure
    {
        public UnitMeasure()
        {
            BillOfMaterials = new HashSet<BillOfMaterial>();
            Products = new HashSet<Product>();
            Products1 = new HashSet<Product>();
            ProductVendors = new HashSet<ProductVendor>();
        }

        [Key]
        [StringLength(3)]
        public string UnitMeasureCode { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<BillOfMaterial> BillOfMaterials { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public virtual ICollection<Product> Products1 { get; set; }

        public virtual ICollection<ProductVendor> ProductVendors { get; set; }
    }
}
