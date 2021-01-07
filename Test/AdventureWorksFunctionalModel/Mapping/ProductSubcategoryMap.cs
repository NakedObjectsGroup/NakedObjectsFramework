using System.Data.Entity.ModelConfiguration;
using AW.Types;

namespace AW.Mapping
{
    public class ProductSubcategoryMap : EntityTypeConfiguration<ProductSubcategory>
    {
        public ProductSubcategoryMap()
        {
            // Primary Key
            HasKey(t => t.ProductSubcategoryID);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("ProductSubcategory", "Production");
            Property(t => t.ProductSubcategoryID).HasColumnName("ProductSubcategoryID");
            Property(t => t.ProductCategoryID).HasColumnName("ProductCategoryID");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken();

            // Relationships
            HasRequired(t => t.ProductCategory)
                .WithMany(t => t.ProductSubcategory)
                .HasForeignKey(d => d.ProductCategoryID);

        }
    }
}
