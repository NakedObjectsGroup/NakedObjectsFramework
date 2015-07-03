namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Production.ProductCategory")]
    public partial class ProductCategory
    {
        public ProductCategory()
        {
            //if (ProductSubcategories == null) {
            //    ProductSubcategories = new HashSet<ProductSubcategory>();
            //}
        }

        public virtual int ProductCategoryID { get; set; }

        [Required]
        [StringLength(50)]
        public virtual string Name { get; set; }

        public virtual Guid rowguid { get; set; }

        public virtual DateTime ModifiedDate { get; set; }

        public virtual ICollection<ProductSubcategory> ProductSubcategories { get; set; }
    }
}
