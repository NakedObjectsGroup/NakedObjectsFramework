using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<ProductCategory> builder)
        {
            builder.HasKey(t => t.ProductCategoryID);
            builder.Ignore(t => t.Name).Ignore(t => t.ModifiedDate).Ignore(t => t.Subcategories);

            // Table & Column Mappings
            builder.ToTable("ProductCategory", "Production");
            builder.Property(t => t.ProductCategoryID).HasColumnName("ProductCategoryID");
            builder.Property(t => t.mappedName).HasColumnName("Name").IsRequired().HasMaxLength(50);
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
