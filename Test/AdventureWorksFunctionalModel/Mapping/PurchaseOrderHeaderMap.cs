using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping {
    public class PurchaseOrderHeaderMap : EntityTypeConfiguration<PurchaseOrderHeader> {
        public PurchaseOrderHeaderMap() {
            // Primary Key
            HasKey(t => t.PurchaseOrderID);

            // Properties
            // Table & Column Mappings
            ToTable("PurchaseOrderHeader", "Purchasing");
            Property(t => t.PurchaseOrderID).HasColumnName("PurchaseOrderID");
            Property(t => t.RevisionNumber).HasColumnName("RevisionNumber");
            Property(t => t.Status).HasColumnName("Status");
            Property(t => t.OrderPlacedByID).HasColumnName("EmployeeID");
            Property(t => t.VendorID).HasColumnName("VendorID");
            Property(t => t.ShipMethodID).HasColumnName("ShipMethodID");
            Property(t => t.OrderDate).HasColumnName("OrderDate");
            Property(t => t.ShipDate).HasColumnName("ShipDate");
            Property(t => t.SubTotal).HasColumnName("SubTotal");
            Property(t => t.TaxAmt).HasColumnName("TaxAmt");
            Property(t => t.Freight).HasColumnName("Freight");
            Property(t => t.TotalDue).HasColumnName("TotalDue").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate"); //.IsConcurrencyToken();

            // Relationships
            HasRequired(t => t.OrderPlacedBy).WithMany().HasForeignKey(t => t.OrderPlacedByID);
            HasRequired(t => t.ShipMethod).WithMany().HasForeignKey(t => t.ShipMethodID);
            HasRequired(t => t.Vendor).WithMany().HasForeignKey(t => t.VendorID);
        }
    }

    public static partial class Mapper {
        public static void Map(this EntityTypeBuilder<PurchaseOrderHeader> builder) {
            builder.HasKey(t => t.PurchaseOrderID);

            // Properties
            // Table & Column Mappings
            builder.ToTable("PurchaseOrderHeader", "Purchasing");
            builder.Property(t => t.PurchaseOrderID).HasColumnName("PurchaseOrderID");
            builder.Property(t => t.RevisionNumber).HasColumnName("RevisionNumber");
            builder.Property(t => t.Status).HasColumnName("Status");
            builder.Property(t => t.OrderPlacedByID).HasColumnName("EmployeeID");
            builder.Property(t => t.VendorID).HasColumnName("VendorID");
            builder.Property(t => t.ShipMethodID).HasColumnName("ShipMethodID");
            builder.Property(t => t.OrderDate).HasColumnName("OrderDate");
            builder.Property(t => t.ShipDate).HasColumnName("ShipDate");
            builder.Property(t => t.SubTotal).HasColumnName("SubTotal");
            builder.Property(t => t.TaxAmt).HasColumnName("TaxAmt");
            builder.Property(t => t.Freight).HasColumnName("Freight");
            builder.Property(t => t.TotalDue).HasColumnName("TotalDue").ValueGeneratedOnAddOrUpdate();
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate"); //.IsConcurrencyToken();

            // Relationships
            builder.HasOne(t => t.OrderPlacedBy).WithMany().HasForeignKey(t => t.OrderPlacedByID);
            builder.HasOne(t => t.ShipMethod).WithMany().HasForeignKey(t => t.ShipMethodID);
            builder.HasOne(t => t.Vendor).WithMany().HasForeignKey(t => t.VendorID);
        }
    }
}