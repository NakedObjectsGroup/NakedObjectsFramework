namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Production.ProductSubcategory")]
    public partial class ProductSubcategory
    {
        public ProductSubcategory()
        {
           // Products = new HashSet<Product>();
        }

        public virtual int ProductSubcategoryID { get; set; }

        public virtual int ProductCategoryID { get; set; }

        [Required]
        [StringLength(50)]
        public virtual string Name { get; set; }

        public virtual Guid rowguid { get; set; }

        public virtual DateTime ModifiedDate { get; set; }

        //public virtual ICollection<Product> Products { get; set; }

        public virtual ProductCategory ProductCategory { get; set; }
    }
}
