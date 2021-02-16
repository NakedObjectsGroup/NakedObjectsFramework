using System.Data.Entity.ModelConfiguration;
using AW.Types;

namespace AW.Mapping
{
    public class ProductCategoryMap : EntityTypeConfiguration<ProductCategory>
    {
        public ProductCategoryMap()
        {
            // Primary Key
            HasKey(t => t.ProductCategoryID);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("ProductCategory", "Production");
            Property(t => t.ProductCategoryID).HasColumnName("ProductCategoryID");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken();
        }
    }
}
