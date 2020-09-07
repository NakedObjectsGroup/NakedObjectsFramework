using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
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
}
