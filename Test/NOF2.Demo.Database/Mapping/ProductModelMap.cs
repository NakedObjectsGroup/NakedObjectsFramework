using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<ProductModel> builder)
        {
            builder.HasKey(t => t.ProductModelID);

            // Properties
            builder.Property(t => t.mappedName)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("ProductModel", "Production");
            builder.Property(t => t.ProductModelID).HasColumnName("ProductModelID");
            builder.Property(t => t.mappedName).HasColumnName("Name");
            builder.Property(t => t.CatalogDescription).HasColumnName("CatalogDescription");
            builder.Property(t => t.mappedInstructions).HasColumnName("Instructions");
            builder.Property(t => t.RowGuid).HasColumnName("rowguid");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
