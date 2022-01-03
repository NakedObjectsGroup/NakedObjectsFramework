using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<ProductVendor> builder)
        {
            // Primary Key
            builder.HasKey(t => new { t.ProductID, t.VendorID });

            // Properties
            builder.Property(t => t.ProductID)
                .ValueGeneratedNever();

            builder.Property(t => t.VendorID)
                .ValueGeneratedNever();

            builder.Property(t => t.UnitMeasureCode)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            // Table & Column Mappings
            builder.ToTable("ProductVendor", "Purchasing");
            builder.Property(t => t.ProductID).HasColumnName("ProductID");
            builder.Property(t => t.VendorID).HasColumnName("BusinessEntityID");
            builder.Property(t => t.mappedAverageLeadTime).HasColumnName("AverageLeadTime");
            builder.Property(t => t.mappedStandardPrice).HasColumnName("StandardPrice");
            builder.Property(t => t.mappedLastReceiptCost).HasColumnName("LastReceiptCost");
            builder.Property(t => t.mappedLastReceiptDate).HasColumnName("LastReceiptDate");
            builder.Property(t => t.mappedMinOrderQty).HasColumnName("MinOrderQty");
            builder.Property(t => t.mappedMaxOrderQty).HasColumnName("MaxOrderQty");
            builder.Property(t => t.mappedOnOrderQty).HasColumnName("OnOrderQty");
            builder.Property(t => t.UnitMeasureCode).HasColumnName("UnitMeasureCode");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false); 

            // Relationships
            builder.HasOne(t => t.Product).WithMany().HasForeignKey(t => t.ProductID);
            builder.HasOne(t => t.UnitMeasure).WithMany().HasForeignKey(t => t.UnitMeasureCode);
            builder.HasOne(t => t.Vendor)
                   .WithMany(t => t.mappedProducts)
                   .HasForeignKey(d => d.VendorID);
        }
    }
}
