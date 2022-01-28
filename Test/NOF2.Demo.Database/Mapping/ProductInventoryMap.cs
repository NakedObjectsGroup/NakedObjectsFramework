using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<ProductInventory> builder)
        {
            builder.HasKey(t => new { t.ProductID, t.LocationID });

            // Properties
            builder.Property(t => t.ProductID)
                   .ValueGeneratedNever();

            builder.Property(t => t.LocationID)
                   .ValueGeneratedNever();

            builder.Property(t => t.mappedShelf)
                   .IsRequired()
                   .HasMaxLength(10);

            // Table & Column Mappings
            builder.ToTable("ProductInventory", "Production");
            builder.Property(t => t.ProductID).HasColumnName("ProductID");
            builder.Property(t => t.LocationID).HasColumnName("LocationID");
            builder.Property(t => t.mappedShelf).HasColumnName("Shelf");
            builder.Property(t => t.mappedBin).HasColumnName("Bin");
            builder.Property(t => t.mappedQuantity).HasColumnName("Quantity");
            builder.Property(t => t.RowGuid).HasColumnName("rowguid");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.Location).WithMany().HasForeignKey(t => t.LocationID);
            builder.HasOne(t => t.Product)
                   .WithMany(t => t.mappedProductInventory)
                   .HasForeignKey(d => d.ProductID);
        }
    }
}
