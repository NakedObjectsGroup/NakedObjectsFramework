using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<ProductSubcategory> builder)
        {
            builder.HasKey(t => t.ProductSubcategoryID);

            builder.Ignore(t => t.mappedName).Ignore(t => t.mappedModifiedDate);

            // Table & Column Mappings
            builder.ToTable("ProductSubcategory", "Production");
            builder.Property(t => t.ProductSubcategoryID).HasColumnName("ProductSubcategoryID");
            builder.Property(t => t.ProductCategoryID).HasColumnName("ProductCategoryID");
            builder.Property(t => t.mappedName).HasColumnName("Name").IsRequired().HasMaxLength(50);
            builder.Property(t => t.RowGuid).HasColumnName("rowguid");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.ProductCategory)
                   .WithMany(t => t.mappedProductSubcategory)
                   .HasForeignKey(d => d.ProductCategoryID);
        }
    }
}
