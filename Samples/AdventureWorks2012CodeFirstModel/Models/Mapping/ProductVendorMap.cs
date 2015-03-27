using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorks2012CodeFirstModel.Models.Mapping
{
    public class ProductVendorMap : EntityTypeConfiguration<ProductVendor>
    {
        public ProductVendorMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ProductID, t.VendorID });

            // Properties
            this.Property(t => t.ProductID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.VendorID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.UnitMeasureCode)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            // Table & Column Mappings
            this.ToTable("ProductVendor", "Purchasing");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.VendorID).HasColumnName("VendorID");
            this.Property(t => t.AverageLeadTime).HasColumnName("AverageLeadTime");
            this.Property(t => t.StandardPrice).HasColumnName("StandardPrice");
            this.Property(t => t.LastReceiptCost).HasColumnName("LastReceiptCost");
            this.Property(t => t.LastReceiptDate).HasColumnName("LastReceiptDate");
            this.Property(t => t.MinOrderQty).HasColumnName("MinOrderQty");
            this.Property(t => t.MaxOrderQty).HasColumnName("MaxOrderQty");
            this.Property(t => t.OnOrderQty).HasColumnName("OnOrderQty");
            this.Property(t => t.UnitMeasureCode).HasColumnName("UnitMeasureCode");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.Product)
                .WithMany(t => t.ProductVendors)
                .HasForeignKey(d => d.ProductID);
            this.HasRequired(t => t.UnitMeasure)
                .WithMany(t => t.ProductVendors)
                .HasForeignKey(d => d.UnitMeasureCode);
            this.HasRequired(t => t.Vendor)
                .WithMany(t => t.ProductVendors)
                .HasForeignKey(d => d.VendorID);

        }
    }
}
