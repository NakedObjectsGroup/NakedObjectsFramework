using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping {
    public class ProductInventoryMap : EntityTypeConfiguration<ProductInventory> {
        public ProductInventoryMap() {
            // Primary Key
            HasKey(t => new { t.ProductID, t.LocationID });

            // Properties
            Property(t => t.ProductID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.LocationID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Shelf)
                .IsRequired()
                .HasMaxLength(10);

            // Table & Column Mappings
            ToTable("ProductInventory", "Production");
            Property(t => t.ProductID).HasColumnName("ProductID");
            Property(t => t.LocationID).HasColumnName("LocationID");
            Property(t => t.Shelf).HasColumnName("Shelf");
            Property(t => t.Bin).HasColumnName("Bin");
            Property(t => t.Quantity).HasColumnName("Quantity");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate"); //.IsConcurrencyToken();

            // Relationships
            HasRequired(t => t.Location).WithMany().HasForeignKey(t => t.LocationID);
            HasRequired(t => t.Product)
                .WithMany(t => t.ProductInventory)
                .HasForeignKey(d => d.ProductID);
        }
    }

    public static partial class Mapper {
        public static void Map(this EntityTypeBuilder<ProductInventory> builder) {
            builder.HasKey(t => new { t.ProductID, t.LocationID });

            // Properties
            builder.Property(t => t.ProductID)
                   .ValueGeneratedNever();

            builder.Property(t => t.LocationID)
                   .ValueGeneratedNever();

            builder.Property(t => t.Shelf)
                   .IsRequired()
                   .HasMaxLength(10);

            // Table & Column Mappings
            builder.ToTable("ProductInventory", "Production");
            builder.Property(t => t.ProductID).HasColumnName("ProductID");
            builder.Property(t => t.LocationID).HasColumnName("LocationID");
            builder.Property(t => t.Shelf).HasColumnName("Shelf");
            builder.Property(t => t.Bin).HasColumnName("Bin");
            builder.Property(t => t.Quantity).HasColumnName("Quantity");
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate"); //.IsConcurrencyToken();

            // Relationships
            builder.HasOne(t => t.Location).WithMany().HasForeignKey(t => t.LocationID);
            builder.HasOne(t => t.Product)
                   .WithMany(t => t.ProductInventory)
                   .HasForeignKey(d => d.ProductID);
        }
    }
}