using System;
using System.Collections.Generic;

namespace AdventureWorksModel.Models
{
    public partial class ProductCategory
    {
        public ProductCategory()
        {
            this.ProductSubcategories = new List<ProductSubcategory>();
        }

        public int ProductCategoryID { get; set; }
        public string Name { get; set; }
        public System.Guid rowguid { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual ICollection<ProductSubcategory> ProductSubcategories { get; set; }
    }
}
