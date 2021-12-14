using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel.Mapping
{
    public class ProductVendorMap : EntityTypeConfiguration<ProductVendor>
    {
        public ProductVendorMap()
        {
            // Primary Key
            HasKey(t => new { t.ProductID, t.VendorID });

            // Properties
            Property(t => t.ProductID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.VendorID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.UnitMeasureCode)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            // Table & Column Mappings
            ToTable("ProductVendor", "Purchasing");
            Property(t => t.ProductID).HasColumnName("ProductID");
            Property(t => t.VendorID).HasColumnName("BusinessEntityID");
            Property(t => t.AverageLeadTime).HasColumnName("AverageLeadTime");
            Property(t => t.StandardPrice).HasColumnName("StandardPrice");
            Property(t => t.LastReceiptCost).HasColumnName("LastReceiptCost");
            Property(t => t.LastReceiptDate).HasColumnName("LastReceiptDate");
            Property(t => t.MinOrderQty).HasColumnName("MinOrderQty");
            Property(t => t.MaxOrderQty).HasColumnName("MaxOrderQty");
            Property(t => t.OnOrderQty).HasColumnName("OnOrderQty");
            Property(t => t.UnitMeasureCode).HasColumnName("UnitMeasureCode");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.Product).WithMany().HasForeignKey(t => t.ProductID);
            HasRequired(t => t.UnitMeasure).WithMany().HasForeignKey(t => t.UnitMeasureCode);
            HasRequired(t => t.Vendor)
                .WithMany(t => t.Products)
                .HasForeignKey(d => d.VendorID);

        }
    }

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
            builder.Property(t => t.AverageLeadTime).HasColumnName("AverageLeadTime");
            builder.Property(t => t.StandardPrice).HasColumnName("StandardPrice");
            builder.Property(t => t.LastReceiptCost).HasColumnName("LastReceiptCost");
            builder.Property(t => t.LastReceiptDate).HasColumnName("LastReceiptDate");
            builder.Property(t => t.MinOrderQty).HasColumnName("MinOrderQty");
            builder.Property(t => t.MaxOrderQty).HasColumnName("MaxOrderQty");
            builder.Property(t => t.OnOrderQty).HasColumnName("OnOrderQty");
            builder.Property(t => t.UnitMeasureCode).HasColumnName("UnitMeasureCode");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false); 

            // Relationships
            builder.HasOne(t => t.Product).WithMany().HasForeignKey(t => t.ProductID);
            builder.HasOne(t => t.UnitMeasure).WithMany().HasForeignKey(t => t.UnitMeasureCode);
            builder.HasOne(t => t.Vendor)
                   .WithMany(t => t.Products)
                   .HasForeignKey(d => d.VendorID);
        }
    }
}
