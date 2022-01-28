using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NOF2.Demo.Model
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<PurchaseOrderHeader> builder)
        {
            builder.HasKey(t => t.PurchaseOrderID);

            // Properties
            // Table & Column Mappings
            builder.ToTable("PurchaseOrderHeader", "Purchasing");
            builder.Property(t => t.PurchaseOrderID).HasColumnName("PurchaseOrderID");
            builder.Property(t => t.mappedRevisionNumber).HasColumnName("RevisionNumber");
            builder.Property(t => t.Status).HasColumnName("Status");
            builder.Property(t => t.OrderPlacedByID).HasColumnName("EmployeeID");
            builder.Property(t => t.VendorID).HasColumnName("VendorID");
            builder.Property(t => t.ShipMethodID).HasColumnName("ShipMethodID");
            builder.Property(t => t.mappedOrderDate).HasColumnName("OrderDate");
            builder.Property(t => t.mappedShipDate).HasColumnName("ShipDate");
            builder.Property(t => t.mappedSubTotal).HasColumnName("SubTotal");
            builder.Property(t => t.mappedTaxAmt).HasColumnName("TaxAmt");
            builder.Property(t => t.mappedFreight).HasColumnName("Freight");
            builder.Property(t => t.mappedTotalDue).HasColumnName("TotalDue").ValueGeneratedOnAddOrUpdate();
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.OrderPlacedBy).WithMany().HasForeignKey(t => t.OrderPlacedByID);
            builder.HasOne(t => t.ShipMethod).WithMany().HasForeignKey(t => t.ShipMethodID);
            builder.HasOne(t => t.Vendor).WithMany().HasForeignKey(t => t.VendorID);
        }
    }
}
